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

namespace BetterWpfControls
{
    public class CollapsibleGridSplitter : GridSplitter
    {
        #region .ctors

        static CollapsibleGridSplitter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CollapsibleGridSplitter), new FrameworkPropertyMetadata(typeof(CollapsibleGridSplitter)));
        }

        public CollapsibleGridSplitter()
        {
            Loaded += new RoutedEventHandler(SmartGridSplitter_Loaded);
            IsVisibleChanged += new DependencyPropertyChangedEventHandler(SmartGridSplitter_IsVisibleChanged);
        }

        #endregion .ctors

        #region Fields

        private UIElement _collapsiblePane;
        private GridDefinition _collapsiblePaneGridInfo;
        private ContentAdorner _adorner;
        private Button _button;

        #endregion Fields

        #region Properties

        public Dock CollapseDirection
        {
            get { return (Dock)GetValue(CollapseDirectionProperty); }
            set { SetValue(CollapseDirectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CollapseDirection.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CollapseDirectionProperty =
            DependencyProperty.Register("CollapseDirection", typeof(Dock), typeof(CollapsibleGridSplitter), new UIPropertyMetadata(Dock.Left));

        public double? Value
        {
            get { return (double?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(double?), typeof(CollapsibleGridSplitter), new UIPropertyMetadata(double.NaN));

        public GridLength? DefaultValue
        {
            get { return (GridLength?)GetValue(DefaultValueProperty); }
            set { SetValue(DefaultValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DefaultValue.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DefaultValueProperty =
            DependencyProperty.Register("DefaultValue", typeof(GridLength?), typeof(CollapsibleGridSplitter), new UIPropertyMetadata(null));

        public bool CollapseOnly
        {
            get { return (bool)GetValue(CollapseOnlyProperty); }
            set { SetValue(CollapseOnlyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CollapseOnly.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CollapseOnlyProperty =
            DependencyProperty.Register("CollapseOnly", typeof(bool), typeof(CollapsibleGridSplitter), new UIPropertyMetadata(false, (s, e) => ((CollapsibleGridSplitter)s).OnCollapseOnlyChanged(e)));

        private void OnCollapseOnlyChanged(DependencyPropertyChangedEventArgs e)
        {
            if (CollapseOnly)
            {
                Cursor = Cursors.Arrow;
            }
        }

        #endregion Properties

        #region Methods

        private void SmartGridSplitter_Loaded(object sender, RoutedEventArgs e)
        {
            var grid = VisualTreeHelper.GetParent(this) as Grid;
            if (grid != null)
            {
                var row = Grid.GetRow(this);
                var column = Grid.GetColumn(this);

                foreach (var item in grid.Children.OfType<UIElement>().Where(el => el != this))
                {
                    switch (CollapseDirection)
                    {
                        case Dock.Bottom:
                            if (Grid.GetRow(item) == row + 1 && Grid.GetColumn(item) == column)
                            {
                                _collapsiblePane = item;
                                _collapsiblePaneGridInfo = new RowGridDefinition() { Def = grid.RowDefinitions[row + 1] };
                            }
                            break;
                        case Dock.Left:
                            if (Grid.GetRow(item) == row && Grid.GetColumn(item) == column - 1)
                            {
                                _collapsiblePane = item;
                                _collapsiblePaneGridInfo = new ColumnGridDefinition() { Def = grid.ColumnDefinitions[column - 1] };
                            }
                            break;
                        case Dock.Right:
                            if (Grid.GetRow(item) == row && Grid.GetColumn(item) == column + 1)
                            {
                                _collapsiblePane = item;
                                _collapsiblePaneGridInfo = new ColumnGridDefinition() { Def = grid.ColumnDefinitions[column + 1] };
                            }
                            break;
                        case Dock.Top:
                            if (Grid.GetRow(item) == row - 1 && Grid.GetColumn(item) == column)
                            {
                                _collapsiblePane = item;
                                _collapsiblePaneGridInfo = new RowGridDefinition() { Def = grid.RowDefinitions[row - 1] };
                            }
                            break;
                        default:
                            break;
                    }
                }
                if (_collapsiblePaneGridInfo != null)
                {
                    if (DefaultValue != null)
                    {
                        _collapsiblePaneGridInfo.Length = DefaultValue;
                        _collapsiblePaneGridInfo.RestoreLength();
                        _collapsiblePaneGridInfo.SaveLength();
                    }

                    if (Value != null)
                    {
                        if (double.IsPositiveInfinity(Value.Value))
                        {
                            _collapsiblePaneGridInfo.SaveLength();
                        }
                        else if (!double.IsNaN(Value.Value))
                        {
                            _collapsiblePaneGridInfo.Length = new GridLength(Value.Value);
                            _collapsiblePaneGridInfo.RestoreLength();
                            _collapsiblePaneGridInfo.SaveLength();
                        }
                    }
                    else
                    {
                        _collapsiblePaneGridInfo.Collapse();
                    }
                    _collapsiblePaneGridInfo.IsCollapsedChanged += new Action<bool>(_collapsiblePaneGridInfo_IsCollapsedChanged);

                    _button = new Button() { Style = ResizeDirection == GridResizeDirection.Columns ? (Style)FindResource("VerticalCollapsibleSpliterButton") : (Style)FindResource("HorizontalCollapsibleSpliterButton") };
                    SetGripCursor();
                    _button.Click += new RoutedEventHandler(button_Click);
                    _adorner = new ContentAdorner(_button, this);
                    if (ResizeDirection == GridResizeDirection.Columns)
                    {
                        _adorner.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                        _adorner.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                    }
                    else
                    {
                        _adorner.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                        _adorner.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                    }
                    if (AdornerLayer.GetAdornerLayer(this) != null)
                    {
                        AdornerLayer.GetAdornerLayer(this).Add(_adorner);
                    }
                }
            }
        }

        private void _collapsiblePaneGridInfo_IsCollapsedChanged(bool obj)
        {
            if (obj)
            {
                if (ResizeDirection == GridResizeDirection.Columns)
                {
                    _adorner.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                }
                else
                {
                    _adorner.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                }
            }
            else
            {
                if (ResizeDirection == GridResizeDirection.Columns)
                {
                    _adorner.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
                }
                else
                {
                    _adorner.VerticalAlignment = System.Windows.VerticalAlignment.Top;
                }
            }
            SetGripCursor();
        }

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            if (CollapseOnly)
            {
                e.Handled = true;
            }
            else
            {
                base.OnPreviewMouseDown(e);
            }
        }

        private void SetGripCursor()
        {
            switch (CollapseDirection)
            {
                case Dock.Bottom:
                    if (_collapsiblePaneGridInfo.IsCollapsed)
                    {
                        _button.Cursor = Cursors.ScrollN;
                    }
                    else
                    {
                        _button.Cursor = Cursors.ScrollS;
                    }
                    break;
                case Dock.Left:
                    if (_collapsiblePaneGridInfo.IsCollapsed)
                    {
                        _button.Cursor = Cursors.ScrollE;
                    }
                    else
                    {
                        _button.Cursor = Cursors.ScrollW;
                    }
                    break;
                case Dock.Right:
                    if (_collapsiblePaneGridInfo.IsCollapsed)
                    {
                        _button.Cursor = Cursors.ScrollW;
                    }
                    else
                    {
                        _button.Cursor = Cursors.ScrollE;
                    }
                    break;
                case Dock.Top:
                    if (_collapsiblePaneGridInfo.IsCollapsed)
                    {
                        _button.Cursor = Cursors.ScrollS;
                    }
                    else
                    {
                        _button.Cursor = Cursors.ScrollN;
                    }
                    break;
                default:
                    break;
            }
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (_collapsiblePaneGridInfo != null)
            {
                _collapsiblePaneGridInfo.Switch();
                UpdateValue();
            }
        }

        void button_Click(object sender, RoutedEventArgs e)
        {
            if (_collapsiblePaneGridInfo != null)
            {
                _collapsiblePaneGridInfo.Switch();
                UpdateValue();
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);

            if (_collapsiblePaneGridInfo != null)
            {
                if (_collapsiblePaneGridInfo.GetAbsoluteValue() > 0)
                {
                    _collapsiblePaneGridInfo.SaveLength();
                    _collapsiblePaneGridInfo.SetCollapsedState(false);
                }
                else
                {
                    _collapsiblePaneGridInfo.SetCollapsedState(true);
                }

                UpdateValue();
            }
        }

        private void UpdateValue()
        {
            if (_collapsiblePaneGridInfo.Length != null && !_collapsiblePaneGridInfo.IsCollapsed)
            {
                Value =
                    _collapsiblePaneGridInfo.Length.Value.IsAuto ?
                    double.PositiveInfinity :
                    _collapsiblePaneGridInfo.Length.Value.Value;
            }
            else
            {
                Value = null;
            }
        }

        private void SmartGridSplitter_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
            {
                if (_adorner != null)
                {
                    _adorner.Visibility = System.Windows.Visibility.Visible;
                }
            }
            else
            {
                if (_adorner != null)
                {
                    _adorner.Visibility = System.Windows.Visibility.Collapsed;
                }
            }
        }

        #endregion Methods

        #region Internal Classes

        private abstract class GridDefinition
        {
            public event Action<bool> IsCollapsedChanged;

            public bool IsCollapsed { get; set; }
            public GridLength? Length { get; set; }

            public abstract double? GetAbsoluteValue();
            public abstract void SaveLength();
            public abstract void NullifyLength();
            public abstract void RestoreLength();

            public virtual void Switch()
            {
                if (GetAbsoluteValue() > 0)
                {
                    Collapse();
                }
                else
                {
                    Restore();
                }
            }

            public void Collapse()
            {
                NullifyLength();
                SetCollapsedState(true);
            }

            public void Restore()
            {
                RestoreLength();
                SetCollapsedState(false);
            }

            public void SetCollapsedState(bool state)
            {
                IsCollapsed = state;
                if (IsCollapsedChanged != null)
                {
                    IsCollapsedChanged(state);
                }
            }
        }

        private abstract class GridDefinition<T> : GridDefinition
            where T : DefinitionBase
        {
            public T Def { get; set; }
        }

        private class ColumnGridDefinition : GridDefinition<ColumnDefinition>
        {
            public override double? GetAbsoluteValue()
            {
                return Def.Width.IsAbsolute ? Def.Width.Value : (Def.Width.IsAuto ? double.PositiveInfinity : (double?)null);
            }

            public override void SaveLength()
            {
                Length = Def.Width;
            }

            public override void NullifyLength()
            {
                Def.Width = new GridLength(0, GridUnitType.Pixel);
            }

            public override void RestoreLength()
            {
                if (Length.HasValue)
                {
                    Def.Width = Length.Value;
                }
            }
        }

        private class RowGridDefinition : GridDefinition<RowDefinition>
        {
            public override double? GetAbsoluteValue()
            {
                return Def.Height.IsAbsolute ? Def.Height.Value : (Def.Height.IsAuto ? double.PositiveInfinity : (double?)null);
            }

            public override void SaveLength()
            {
                Length = Def.Height;
            }

            public override void NullifyLength()
            {
                Def.Height = new GridLength(0, GridUnitType.Pixel);
            }

            public override void RestoreLength()
            {
                if (Length.HasValue)
                {
                    Def.Height = Length.Value;
                }
            }
        }

        #endregion Internal Classes
    }
}
