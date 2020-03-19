# Shutdown Timer Classic

Shutdown Timer Classic or otherwise known as ShutdownTimerWin32 is a small little Windows application that allows you to set a timer which will shutdown, restart, hibernate, sleep or lock your PC.

![Screenshot of the main menu](Images/Menu.png)

I also have a UWP based version of this project which will be available on the Microsoft Store when it's ready but that will take some time. Once the project is ready I will update this document and my website.

# Usage

Shutdown Timer is a very simple application hence it is easy to use.
Just choose a power action from the drop-down menu and then dial in the time span you want. If the counter reaches zero the chosen power action will be executed.

By default, the countdown window will be always on top of every other window so you don't forget that you have an active shutdown timer. The countdown can also be hidden by selecting the "Run in background" checkbox under the dropdown menu.

![Screenshot of the main menu with extended combobox](Images/Menu2.png)

# Colors

The countdown window has 4 different background colors to visualize the time left and one animation to draw your attention when the time is about to run out, in case you forget about the shutdown timer.
Here is a quick overview:

| Time left     | Color         | Animated  |
| ------------- | ------------- | --------- |
| > 30 min.     | Green         | No        |
| 30 - 10 min.  | Yellow        | No        |
| 10 - 1 min.   | Orange        | No        |
| < 1min.       | Red / Black   | Yes       |

![Screenshot of countdown window with green background](Images/CountdownGreen.png)
![Screenshot of countdown window with yellow background](Images/CountdownYellow.png)
![Screenshot of countdown window with orange background](Images/CountdownOrange.png)
![Screenshot of countdown window with red background](Images/CountdownRed.png)
![Screenshot of countdown window with black background](Images/CountdownBlack.png)

# Behavior

## Shutdown sequence

When a shutdown is executed all windows will be closed, regardless if they resist or not so any unsaved work will be gone! This is done to be sure your PC shuts down and can not be stopped by another process. Please note that I am not liable for any data loss because you didn't save that extremely important document and now it's gone. That is your fault!

## Canceling the shutdown

As long as the timer has not reached zero, you can cancel the countdown at any time by simply pressing the close button (that big X in the top right corner). A dialog will then pop up asking you if you want to cancel. If you choose so, the countdown will be immediately canceled and another message will tell you that the countdown was successfully canceled and that the application will close after clicking OK. At this point, the countdown has already stopped and you have all the time you want before clicking ok, which results in the application closing itself.

If you are running it in the background, then you can go to the notification area (click on the arrow on the right side of your taskbar) and right-click on the application icon. This will bring up a menu with the option "Stop and exit". Clicking this will cancel the shutdown and tell you about the cancellation using a message box.

## Logging and Privacy

The application is not connected to the internet and does not log any user data when you are using the GitHub release. If you are using the Microsoft Store release, then the Store will monitor basic usage and crashes. The application also stores a small boolean on your system, so it knows if you have seen the license message or not (this is checked on every launch and in case you have not seen the message it will be shown before the main menu pops up).
