using Godot;
using Godot.Collections;

using System;
using System.IO;
using System.Text;
using System.Threading;

namespace NagaisoraFramework.LogSystem
{
    public partial class Log : Logger
    {
        public Stream Stream;
        public StreamWriter Writer;
        public OutLivel OutLivel;

        public Log(OutLivel outLivel, Stream stream)
        {
            Stream = stream;
            Writer = new(Stream, Encoding.UTF8);

            OutLivel = outLivel;
        }

        public Log(OutLivel outLivel, string filePath = "")
        {
            if (filePath is null || filePath == string.Empty)
            {
                throw new ArgumentNullException(nameof(filePath));
            }

			Stream = new FileStream(filePath, FileMode.Create, System.IO.FileAccess.ReadWrite, FileShare.Read);
            Writer = new(Stream, Encoding.UTF8);

            OutLivel = outLivel;
		}

        public override void _LogMessage(string message, bool error)
        {
            base._LogMessage(message, error);

            LogType logType = error ? LogType.Error : LogType.Information;

            if (error)
            {
                return;
            }
            
            LogOut(message.Trim('\r', '\n'), logType);
        }

        public override void _LogError(string function, string file, int line, string code, string rationale, bool editorNotify, int errorType, Array<ScriptBacktrace> scriptBacktraces)
        {
            base._LogError(function, file, line, code, rationale, editorNotify, errorType, scriptBacktraces);

            LogType LogType = LogType.Warning;

            switch ((ErrorType)errorType)
            {
                case ErrorType.Error:
                    LogType = LogType.Error;
                    break;
                case ErrorType.Warning:
                    LogType = LogType.Warning;
                    break;
                case ErrorType.Script:
                    LogType = LogType.Error;
                    break;
                case ErrorType.Shader:
                    LogType = LogType.Error;
                    break;
            }

            string EditorNotify = editorNotify ? "EditorNotify " : "";


			if (LogType is LogType.Warning)
            {
                LogOut($"{EditorNotify}{function} -> {file}:{line} >> {code} {rationale.Trim('\r', '\n')}", LogType);
            }
            else
            {
                LogOut($"{EditorNotify}{(ErrorType)errorType} {function} -> {file}:{line} >> {code} {rationale.Trim('\r', '\n')}", LogType);
			}
		}

		public void LogOut(Exception exception, LogType logType)
        {
            LogOut(exception.ToString(), logType);
        }

        public void LogOut(string message, LogType logType)
        {
            if (OutLivel == OutLivel.None)
            {
                return;
            }

            switch (logType)
            {
                case LogType.Fatal:
					Console.ForegroundColor = ConsoleColor.DarkRed;
					break;
				case LogType.Error:
                    if (OutLivel == OutLivel.Fatal)
                    {
                        return;
                    }
					Console.ForegroundColor = ConsoleColor.Red;
					break;
                case LogType.Warning:
					if (OutLivel == OutLivel.Error || OutLivel == OutLivel.Fatal)
					{
						return;
					}
					Console.ForegroundColor = ConsoleColor.DarkYellow;
					break;
				case LogType.Information:
					if (OutLivel == OutLivel.Warning || OutLivel == OutLivel.Error || OutLivel == OutLivel.Fatal)
					{
						return;
					}
					break;
				case LogType.Debug:
					if (OutLivel == OutLivel.Information || OutLivel == OutLivel.Warning || OutLivel == OutLivel.Error || OutLivel == OutLivel.Fatal)
					{
						return;
					}
					Console.ForegroundColor = ConsoleColor.DarkGray;
					break;
			}

            char LT = char.MinValue;

            switch (logType)
            {
				case LogType.Debug:
					LT = 'D';
					break;
				case LogType.Information:
                    LT = 'I';
                    break;
                case LogType.Warning:
                    LT = 'W';
                    break;
                case LogType.Error:
                    LT = 'E';
                    break;
                case LogType.Fatal:
                    LT = 'F';
                    break;
                case LogType.Stop:
                    LT = 'P';
                    break;
            }

            string OutMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff}] [{LT}] {message}";

            Console.WriteLine(OutMessage);
            Writer?.WriteLine(OutMessage);
            Writer?.Flush();
            Stream?.Flush();

            Console.ForegroundColor = ConsoleColor.White;
        }

        public void LogFileSave()
        {
            Writer?.Close();
            Stream?.Close();
        }
	}
}
