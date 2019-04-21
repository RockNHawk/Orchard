﻿namespace Devq.ErrorLog.Models
{
    public class LogItem
    {
        public string Text { get; set; }
        public string Date { get; set; }
        public string Preview {
            get { return Text.Substring(0, 75) + "..."; }
        }
    }
}