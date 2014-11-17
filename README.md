A C# Winforms translation of Trystan's AsciiPanel.

To quote from Trystan's README:

AsciiPanel
---------

AsciiPanel simulates a code page 437 ASCII terminal display. It supports all
256 characters of codepage 437, arbitrary foreground colors, arbitrary
background colors, and arbitrary terminal sizes.

The default terminal size is 80x24 characters and default colors matching the
Windows Command Prompt are provided.

This should be useful to roguelike developers.

---------

To use AsciiPanelSharp, include the project in your solution. Create a form that inherits from AsciiPanel and you can then use the Write method in order to write characters onto the terminal emulator.
