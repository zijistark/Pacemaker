using System.Collections.Generic;

using TaleWorlds.Library;

namespace Pacemaker
{
    internal class GameLogBase
    {
        public virtual void Info(string s) { }
        public virtual void Info(List<string> l) { }
        public virtual void Debug(string s) { }
        public virtual void Debug(List<string> l) { }
        public virtual void NotifyBad(string s) { }
        public virtual void NotifyBad(List<string> l) { }
        public virtual void NotifyNeutral(string s) { }
        public virtual void NotifyNeutral(List<string> l) { }
        public virtual void NotifyGood(string s) { }
        public virtual void NotifyGood(List<string> l) { }

        public virtual void Print(string text, Color color, bool isDebug = false, bool onlyDisplay = false) { }
        public virtual void Print(List<string> lines, Color color, bool isDebug = false, bool onlyDisplay = false) { }

        public virtual void ToFile(string line, bool isDebug = false) { }
        public virtual void ToFile(List<string> lines, bool isDebug = false) { }
    }
}
