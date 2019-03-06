﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Runtime.CompilerServices;

namespace Command_Server
{
    public enum LogLevel
    {
        Trace,
        Debug,
        Info,
        Warn,
        Error
    }

    public static class Logger
    {
        private static readonly string LoggerName = "Command-Server";
        public static LogLevel LogLevel = LogLevel.Info;
        private static readonly ConsoleColor DefaultFgColor = ConsoleColor.Gray;

        private static void ResetForegroundColor()
        {
            Console.ForegroundColor = DefaultFgColor;
        }

        public static void Trace(string text,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)//params object[] args)
        {
            if (LogLevel > LogLevel.Trace)
            {
                return;
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"[{Path.GetFileName(file)}_{member}({line}) @ {DateTime.Now.ToString("HH:mm")} - Info] {text}");
            //Console.ForegroundColor = ConsoleColor.Cyan;
            //Console.WriteLine("[" + LoggerName + " @ " + DateTime.Now.ToString("HH:mm") + " - Trace] " + String.Format(format, args));
            ResetForegroundColor();
        }

        public static void Debug(string text,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)//params object[] args)
        {
            if (LogLevel > LogLevel.Debug)
            {
                return;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{Path.GetFileName(file)}_{member}({line}) @ {DateTime.Now.ToString("HH:mm")} - Info] {text}");
            //Console.ForegroundColor = ConsoleColor.Green;
            //Console.WriteLine("[" + LoggerName + " @ " + DateTime.Now.ToString("HH:mm") + " - Debug] " + String.Format(format, args));
            ResetForegroundColor();
        }

        public static void Info(string text,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)//params object[] args)
        {
            if (LogLevel > LogLevel.Info)
            {
                return;
            }

            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"[{Path.GetFileName(file)}_{member}({line}) @ {DateTime.Now.ToString("HH:mm")} - Info] {text}");
            //Console.WriteLine("[" + LoggerName + " @ " + DateTime.Now.ToString("HH:mm") + " - Info] " + String.Format(format, args));
            //Console.WriteLine("{0}_{1}({2}): {3}", Path.GetFileName(file), member, line, format);
            ResetForegroundColor();
        }

        public static void Warning(string text,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)//params object[] args)
        {
            if (LogLevel > LogLevel.Warn)
            {
                return;
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[{Path.GetFileName(file)}_{member}({line}) @ {DateTime.Now.ToString("HH:mm")} - Warning] {text}");
            //Console.ForegroundColor = ConsoleColor.Yellow;
            //Console.WriteLine("[" + LoggerName + " @ " + DateTime.Now.ToString("HH:mm") + " - Warning] " + String.Format(format, args));
            ResetForegroundColor();
        }

        public static void Error(string text,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)//params object[] args)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{Path.GetFileName(file)}_{member}({line}) @ {DateTime.Now.ToString("HH:mm")} - Error] {text}");
            
            //Console.WriteLine("[" + LoggerName + " @ " + DateTime.Now.ToString("HH:mm") + " - Error] " + String.Format(format, args));
            ResetForegroundColor();
        }

        public static void Exception(string text,
            Exception e,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            //Console.WriteLine("[" + LoggerName + " @ " + DateTime.Now.ToString("HH:mm") + "] " + String.Format("{0}-{1}-{2}\n{3}", text, e.GetType().FullName, e.Message, e.StackTrace));
            Console.WriteLine($"[{Path.GetFileName(file)}_{member}({line}) @ {DateTime.Now.ToString("HH:mm")} - Info] {text} - {e.GetType().FullName}-{e.Message}\n{e.StackTrace}");
            ResetForegroundColor();
        }

        public static string EscapeBraces(this string str)
        {
            return str.Replace("{", "{{").Replace("}", "}}");
        }
    }
}
