﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SequencePresenter
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static string DirectoryToTraverse { get; private set; }
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            DirectoryToTraverse = e.Args.FirstOrDefault();
        }
    }
}
