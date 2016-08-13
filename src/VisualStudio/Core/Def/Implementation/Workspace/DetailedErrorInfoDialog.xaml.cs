﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.VisualStudio.LanguageServices.Implementation
{
    internal partial class DetailedErrorInfoDialog : DialogWindow
    {
        private readonly string _errorInfo;

        internal DetailedErrorInfoDialog(string title, string errorInfo)
        {
            InitializeComponent();
            _errorInfo = errorInfo;
            this.Title = title;
            stackTraceText.AppendText(errorInfo);
            this.CopyButton.Content = ServicesVSResources.Copy_to_clipboard;
            this.CloseButton.Content = ServicesVSResources.Close;
        }

        private void CopyMessageToClipBoard(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Clipboard.SetText(_errorInfo);
            }
            catch (Exception)
            {
                // rdpclip.exe not running in a TS session, ignore
            }
        }

        private void CloseWindow(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
