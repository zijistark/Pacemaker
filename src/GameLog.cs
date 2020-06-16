using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using TaleWorlds.Core;
using TaleWorlds.Library;

namespace Pacemaker
{
	public class GameLog : GameLogBase
	{
		private const string BeginMultiLine      = @"=======================================================================================================================\";
		private const string BeginMultiLineDebug = @"===================================================   D E B U G   =====================================================\";
		private const string EndMultiLine        = @"=======================================================================================================================/";

		public readonly string Module;
		public readonly string LogDir;
		public readonly string LogFile;
		public readonly string LogPath;

		private bool _lastMsgWasMultiLine = false;
		private TextWriter _fileWriter;

		protected TextWriter Writer { get => _fileWriter; set => _fileWriter = value; }

		private static readonly Color colorWhite = Color.FromUint(0x00FFFFFF);
		private static readonly Color colorMagenta = Color.FromUint(0x00FF007F);
		private static readonly Color colorRed = Color.FromUint(0x00FF0000);
		private static readonly Color colorSkyBlue = Color.FromUint(0x00007CD7);
		private static readonly Color colorForestGreen = Color.FromUint(0x00FF007F);

		public static Color ColorWhite => colorWhite;
		public static Color ColorMagenta => colorMagenta;
		public static Color ColorRed => colorRed;
		public static Color ColorSkyBlue => colorSkyBlue;
		public static Color ColorForestGreen => colorForestGreen;

		public override void Info(string text)                { Print(text, ColorWhite); }
		public override void Info(List<string> text)          { Print(text, ColorWhite); }
		public override void Debug(string text)               { Print(text, ColorMagenta, true); }
		public override void Debug(List<string> text)         { Print(text, ColorMagenta, true); }
		public override void NotifyBad(string text)           { Print(text, ColorRed); }
		public override void NotifyBad(List<string> text)     { Print(text, ColorRed); }
		public override void NotifyNeutral(string text)       { Print(text, ColorSkyBlue); }
		public override void NotifyNeutral(List<string> text) { Print(text, ColorSkyBlue); }
		public override void NotifyGood(string text)          { Print(text, ColorForestGreen); }
		public override void NotifyGood(List<string> text)    { Print(text, ColorForestGreen); }

		public override void Print(string text, Color color, bool isDebug = false, bool onlyDisplay = false)
		{
			InformationManager.DisplayMessage( new InformationMessage(text, color) );

			if (!onlyDisplay)
				ToFile(text, isDebug);
		}

		public override void Print(List<string> lines, Color color, bool isDebug = false, bool onlyDisplay = false)
		{
			foreach (string text in lines)
				InformationManager.DisplayMessage( new InformationMessage(text, color) );

			if (!onlyDisplay)
				ToFile(lines, isDebug);
		}

		public override /* async */ void ToFile(string line, bool isDebug = false)
		{
			if (Writer == null) return;

			_lastMsgWasMultiLine = false;
			Writer.WriteLine(isDebug ? $">> {line}" : line);
			//await Writer.FlushAsync();
			Writer.Flush();
		}

		public override /* async */ void ToFile(List<string> lines, bool isDebug = false)
		{
			if (Writer == null || lines.Count == 0) return;

			if (lines.Count == 1)
			{
				ToFile(lines[0], isDebug);
				return;
			}

			if (!_lastMsgWasMultiLine)
				Writer.WriteLine(isDebug ? BeginMultiLineDebug : BeginMultiLine);

			_lastMsgWasMultiLine = true;

			foreach (string line in lines)
				Writer.WriteLine(line);

			Writer.WriteLine(EndMultiLine);
			// await Writer.FlushAsync();
			Writer.Flush();
		}

		public GameLog(string moduleName, bool truncate = false, string logName = null)
		{
			if (moduleName.IsStringNoneOrEmpty())
				throw new ArgumentNullException(nameof(moduleName));

			var userDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Mount and Blade II Bannerlord");

			Module = $"{moduleName}.{this.GetType().Name}";

			LogDir = Path.Combine(userDir, "Logs");

			if (logName.IsStringNoneOrEmpty())
				LogFile = $"{moduleName}.log";
			else
				LogFile = $"{moduleName}.{logName}.log";

			LogPath = Path.Combine(LogDir, LogFile);
			Directory.CreateDirectory(LogDir);
			var existed = File.Exists(LogPath);

			try
			{
				// Give it a 64KiB buffer so that it will essentially never block on interim WriteLine calls before asynchronously flushing to disk:
				Writer = TextWriter.Synchronized( new StreamWriter(LogPath, !truncate, Encoding.UTF8, (1 << 16)) );
			}
			catch (Exception e)
			{
				Console.WriteLine($"================================  EXCEPTION  ================================");
				Console.WriteLine($"{Module}: Failed to create StreamWriter!");
				Console.WriteLine($"Path: {LogPath}");
				Console.WriteLine($"Truncate: {truncate}");
				Console.WriteLine($"Preexisting Path: {existed}");
				Console.WriteLine($"Exception Information:");
				Console.WriteLine($"{e}");
				Console.WriteLine($"=============================================================================");
				throw;
			}

			Writer.NewLine = "\n";

			var msg = new List<string>() {
				$"{Module} created at: {DateTimeOffset.Now:yyyy/MM/dd H:mm zzz}",
				$"Working Path: {Directory.GetCurrentDirectory()}",
				$"Assembly:     {this.GetType().AssemblyQualifiedName}",
			};

			if (existed && !truncate)
			{
				Writer.WriteLine();
				Writer.WriteLine();
				msg.Add("NOTE: Any prior log messages in this file may have no relation to this session.");
			}

			ToFile(msg, true);
		}

		~GameLog()
		{
			try
			{
				Writer.Dispose();
			}
			catch (Exception)
			{
				// at least we tried.
			}
		}
	}
}
