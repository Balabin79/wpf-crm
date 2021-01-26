using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace Dental.Infrastructures.Commands
{
    public static class TreeCommands
    {
        public static RoutedCommand CopyRowCommand { get; } = new RoutedCommand();
    }
}
