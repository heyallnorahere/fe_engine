using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FEEngine.Math;
using FEEngine.Util;
using FEEngine.UI;

namespace FEEngine
{
    public class Tests
    {
        private delegate bool Test();
        private struct Entry<T>
        {
            public T data;
            public string name;
        }
        private static List<Entry<T>> AppendEntry<T>(List<Entry<T>> original, T data, string name)
        {
            List<Entry<T>> copy = original;
            Entry<T> entry = new Entry<T>();
            entry.data = data;
            entry.name = name;
            copy.Add(entry);
            return copy;
        }
        private static List<Entry<Test>> GenerateTestList()
        {
            List<Entry<Test>> tests = new List<Entry<Test>>();
            tests = AppendEntry(tests, TestsImpl.Registers, "Tests.Registers");
            tests = AppendEntry(tests, TestsImpl.Map, "Tests.Map");
            tests = AppendEntry(tests, TestsImpl.Unit, "Tests.Unit");
            return tests;
        }
        /// <summary>This function tests all necessary C# classes in the FEEngine namespace.</summary>
        /// <returns>If the tests succeeded</returns>
        public static bool Run()
        {
            List<Entry<Test>> tests = GenerateTestList();
            bool succeeded = true;
            foreach (Entry<Test> test in tests) {
#if FEENGINE_DEBUG
                Logger.Print($"Running test: {test.name}...");
#endif
                if (!test.data())
                {
#if FEENGINE_DEBUG
                    Logger.Print($"Test {test.name} failed!", Renderer.Color.RED);
#endif
                    succeeded = false;
                    break;
                }
                else
                {
#if FEENGINE_DEBUG
                    Logger.Print($"Test {test.name} succeeded!");
#endif
                }
            }
            return succeeded;
        }
        // TESTS
        private struct TestsImpl {
            private static bool TestRegister<T>(bool testCount = true) where T : RegisteredObject<T>, new()
            {
                ObjectRegister<T> register = ObjectRegistry.GetRegister<T>();
                if (register == null)
                {
                    return false;
                }
                return testCount ? (register.Count > 0) : true;
            }
            private static bool IsInside(Vec2<int> a, Vec2<int> b)
            {
                if (a.X >= b.X)
                {
                    return false;
                }
                if (a.X < 0)
                {
                    return false;
                }
                if (a.Y >= b.Y)
                {
                    return false;
                }
                if (a.Y < 0)
                {
                    return false;
                }
                return true;
            }
            public static bool Registers()
            {
                if (!TestRegister<Map>()) return false;
                if (!TestRegister<Unit>()) return false;
                if (!TestRegister<Item>()) return false;
                if (!TestRegister<InputMapper>()) return false;
                if (!TestRegister<UIController>()) return false;
                if (!TestRegister<Menu>(false)) return false;
                return true;
            }
            public static bool Map()
            {
                Map map = FEEngine.Map.GetMap();
                // this should not happen, but just in case
                if (map == null)
                {
                    return false;
                }
                return map.GetUnitCount() > 0;
            }
            public static bool Unit()
            {
                Map map = FEEngine.Map.GetMap();
                // should exist, checked in previous test
                Unit unit = map.GetUnit(0);
                ObjectRegister<Unit> unitRegister = ObjectRegistry.GetRegister<Unit>();
                if (unit.Index >= unitRegister.Count)
                {
                    return false;
                }
                if (!IsInside(unit.Position, map.GetSize()))
                {
                    return false;
                }
                return true;
            }
        }
    }
}
