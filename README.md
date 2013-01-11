#What is it?
BetterWpfControls is a collection of WPF controls and components. Some of them are brand-new, and some are enhanced versions of existing controls which come with .NET.

##What's in the box
###TreeListBox
Sometimes you really have to show giant trees on the screen, and virtualization story for WPF TreeView control isn't great. What TreeViewList does is:

1.  It converts trees to plain lists on-the-fly
2.  Then it reuses virtualization provided by ListBox/VirtualizingStackPanel.

###TabControl
What's wrong with default WPF TabControl? Well, nothing...except this: 
![](http://i3.codeplex.com/Download?ProjectName=betterwpfcontrols&DownloadId=325765)

Here is what BetterTabControl gives you:
*Scrolling. If there are too many tabs, you'll see "scroll next/scroll previous" buttons 
![](http://i3.codeplex.com/Download?ProjectName=betterwpfcontrols&DownloadId=325767)
*Quick links to all tabs. Even if a tab is not visible, you can jump to it: 
![](http://i3.codeplex.com/Download?ProjectName=betterwpfcontrols&DownloadId=325768)
*Locked tabs. If some tabs are too important to be scrolled, lock them (like "Summary" tab on the screenshot):
![](http://i3.codeplex.com/Download?ProjectName=betterwpfcontrols&DownloadId=325769)

### Scrollable Panel
Custom panel which scrolls its content when there is not enough space to show everything:
![](http://i3.codeplex.com/Download?ProjectName=betterwpfcontrols&DownloadId=325368)

### Resizable Panel
Custom panel which nicely resizes its children when there is not enough space to show everything.
This is how items in the sample app look like when there is enough space to show them all:

![](http://i3.codeplex.com/Download?ProjectName=betterwpfcontrols&DownloadId=343928)

When you reduce the width of the window, panel resizes items, starting with the longest ones:

![](http://i3.codeplex.com/Download?ProjectName=betterwpfcontrols&DownloadId=343929)

In the end all items have the size:

![](http://i3.codeplex.com/Download?ProjectName=betterwpfcontrols&DownloadId=343930)

###MenuButton
Use a menu button when you need a menu for a small set of related commands.

![](http://i3.codeplex.com/Download?ProjectName=betterwpfcontrols&DownloadId=325758)

###SplitButton
Use a split button to consolidate a set of variations of a command, especially when one of the commands is used most of the time.

![](http://i3.codeplex.com/Download?ProjectName=betterwpfcontrols&DownloadId=325759)

###Other controls
* Auto-complete texbox - shows hints to users
* ImageButton - a chrome-less button which uses a given image as its content 
* CollapsibleGridSplitter - can collapse grid's row (or column) in one click
* ContentAdorner - an implementation of Adorner which accepts a given UIElement as its visual element
* ContentBox - a button which shows additinal content in a popup on a click
