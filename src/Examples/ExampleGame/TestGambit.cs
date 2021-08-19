using FEEngine;
using FEEngine.Math;

namespace ExampleGame
{
    public class TestGambit : Gambit
    {
        public TestGambit() : base(GambitType.PhysicalAttack) { }
        public override int MaxUses => 2;
        public override IVec2<int> Range => new Vec2I(1);
        protected override void Use(GambitArgs args)
        {
            // todo: add attack system to gambits or something
            Logger.Print(Color.Green, $"{nameof(TestGambit)} triggered or something lol");
        }
    }
}
