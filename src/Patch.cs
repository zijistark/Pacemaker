using HarmonyLib;

using System;

namespace Pacemaker
{
    internal class Patch
    {
        private readonly Type _type;
        private readonly Reflect.Method _targetRM;
        private readonly Reflect.Method _patchRM;
        private readonly int _priority;

        public Patch(Type type, Reflect.Method targetMethod, Reflect.Method patchMethod, int priority = -1)
        {
            _type = type;
            _targetRM = targetMethod;
            _patchRM = patchMethod;
            _priority = priority;
        }

        public void Apply(Harmony harmony)
        {
            var harmonyMethod = new HarmonyMethod(_patchRM.MethodInfo, _priority);

            var mi = harmony.Patch(_targetRM.MethodInfo,
                                   _type == Type.Prefix ? harmonyMethod : null,
                                   _type == Type.Postfix ? harmonyMethod : null);

            if (mi is null)
                throw new Exception($"Harmony patching failed: {this}!");
        }

        public override string ToString() => $"{Enum.GetName(typeof(Type), _type)} patch of "
                                           + $"{_targetRM.PrettyName} in type {_targetRM.Type.FullName}";

        public enum Type
        {
            Prefix = 0,
            Postfix = 1,
        }
    }
}
