using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Controls.Primitives;
using BetterWpfControls.Components;

namespace BetterWpfControls
{
    public class AutoCompleteTextBox : ComboBox
    {
        #region .ctors

        static AutoCompleteTextBox()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(AutoCompleteTextBox), new FrameworkPropertyMetadata(typeof(AutoCompleteTextBox)));
        }

        public AutoCompleteTextBox()
        {
            AddItemCommand = new DelegateCommand<string>(AddItem);
        }

        #endregion .ctors

        #region Fields

        private TextBox _txtBox;
        private ItemsPresenter _itemsPresenter;
        private Popup _popup;
        private bool _hasFocus;
        private bool _suppressTextChange;

        #endregion Fields

        #region Properties

        public ICommand RequestAutoCompleteItemsCommand
        {
            get { return (ICommand)GetValue(RequestAutoCompleteItemsCommandProperty); }
            set { SetValue(RequestAutoCompleteItemsCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for RequestAutoCompleteItemsCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RequestAutoCompleteItemsCommandProperty =
            DependencyProperty.Register("RequestAutoCompleteItemsCommand", typeof(ICommand), typeof(AutoCompleteTextBox), new UIPropertyMetadata(null));

        public ICommand ExecuteDefaultActionCommand
        {
            get { return (ICommand)GetValue(ExecuteDefaultActionCommandProperty); }
            set { SetValue(ExecuteDefaultActionCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ExecuteDefaultActionCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ExecuteDefaultActionCommandProperty =
            DependencyProperty.Register("ExecuteDefaultActionCommand", typeof(ICommand), typeof(AutoCompleteTextBox), new UIPropertyMetadata(null));

        public ICommand AddItemCommand
        {
            get { return (ICommand)GetValue(AddItemCommandProperty); }
            set { SetValue(AddItemCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AddItemCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AddItemCommandProperty =
            DependencyProperty.Register("AddItemCommand", typeof(ICommand), typeof(AutoCompleteTextBox), new UIPropertyMetadata(null));

        #endregion Properties

        #region Methods

        protected override void OnGotFocus(RoutedEventArgs e)
        {
            base.OnGotFocus(e);
            MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            _txtBox = Template.FindName("PART_EditableTextBox", this) as TextBox;
            if (_txtBox != null)
            {
                _txtBox.GotFocus += new RoutedEventHandler(_txtBox_GotFocus);
                _txtBox.LostFocus += new RoutedEventHandler(_txtBox_LostFocus);
                _txtBox.TextChanged += new TextChangedEventHandler(_txtBox_TextChanged);
            }
            _itemsPresenter = Template.FindName("ItemsPresenter", this) as ItemsPresenter;
            _popup = Template.FindName("Popup", this) as Popup;
            if (_popup != null)
            {
                _popup.MouseMove += new MouseEventHandler(_popup_MouseMove);
                _popup.MouseEnter += new MouseEventHandler(_popup_MouseEnter);
            }
        }

        void _popup_MouseEnter(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            _txtBox.Focus();
        }

        void _popup_MouseMove(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            _txtBox.Focus();
        }

        void _txtBox_GotFocus(object sender, RoutedEventArgs e)
        {
            _hasFocus = true;
        }

        void _txtBox_LostFocus(object sender, RoutedEventArgs e)
        {
            _hasFocus = false;
        }

        void _txtBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_suppressTextChange) return;

            Text = _txtBox.Text;

            if (_hasFocus)
            {
                if ((Text ?? string.Empty).Length == 0)
                {
                    IsDropDownOpen = false;
                    _txtBox.Focus();
                }
                else
                {
                    IsDropDownOpen = true;

                    var filter = GetFilter();

                    if (RequestAutoCompleteItemsCommand != null && RequestAutoCompleteItemsCommand.CanExecute(filter))
                    {
                        var fullText = Text;

                        _suppressTextChange = true;
                        RequestAutoCompleteItemsCommand.Execute(filter);
                        _suppressTextChange = false;

                        _txtBox.Text = fullText;
                    }
                }
            }
        }

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnPreviewKeyDown(e);

            if (e.Key == Key.Home || e.Key == Key.End)
            {
                if (!_txtBox.IsFocused)
                {
                    e.Handled = true;
                }
            }
            if (e.Key == Key.Down && _txtBox.IsFocused)
            {
                var item = ItemContainerGenerator.ContainerFromIndex(0) as ComboBoxItem;
                if (item != null)
                {
                    item.Focus();
                    e.Handled = true;
                }
            }
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            base.OnKeyUp(e);

            if (e.Key == Key.Enter)
            {
                if (!IsDropDownOpen)
                {
                    if (ExecuteDefaultActionCommand != null && ExecuteDefaultActionCommand.CanExecute(_txtBox.Text))
                    {
                        ExecuteDefaultActionCommand.Execute(_txtBox.Text);
                    }
                }
            }
        }

        private string GetFilter()
        {
            var filter = _txtBox.Text;
            var index = filter.LastIndexOfAny(new char[] { ';', ',' });
            if (index > 0)
            {
                filter = filter.Substring(index + 1);
            }
            return filter.TrimEnd('*').TrimStart('!');
        }

        private void AddItem(string item)
        {
            IsDropDownOpen = false;

            var filter = GetFilter();
            if (filter != null && item != null)
            {
                _suppressTextChange = true;
                Text = string.Format("{0}{1}", _txtBox.Text.Substring(0, _txtBox.Text.Length - filter.Length), item);
                _suppressTextChange = false;
                _txtBox.Focus();
                _txtBox.Select(Text.Length, 0);
            }
        }

        protected override void OnDropDownOpened(EventArgs e)
        {
            base.OnDropDownOpened(e);
        }

        protected override void OnDropDownClosed(EventArgs e)
        {
            base.OnDropDownClosed(e);

            try
            {
                var w = this.GetParent<Window>();
                if (w == null) return;

                var p = Mouse.GetPosition(w);
                if (p != null)
                {
                    var c = w.InputHitTest(p) as DependencyObject;
                    while (c != null && !(c is ButtonBase) && !(c is FlowDocument) && !(c is FrameworkContentElement))
                    {
                        c = VisualTreeHelper.GetParent(c);
                    }
                    if (c is ButtonBase)
                    {
                        var btn = c as ButtonBase;
                        btn.RaiseEvent(new RoutedEventArgs(ButtonBase.ClickEvent, btn));
                        if (btn.Command != null && btn.Command.CanExecute(btn.CommandParameter))
                        {
                            btn.Command.Execute(btn.CommandParameter);
                        }
                    }
                }
            }
            catch
            {
            }
        }

        #endregion Methods
    }
}