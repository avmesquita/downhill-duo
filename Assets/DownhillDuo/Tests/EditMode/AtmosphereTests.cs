using DownhillDuo.World;
using NUnit.Framework;

namespace DownhillDuo.Tests
{
    public class AtmosphereTests
    {
        // Valores de referência ISA (kg/m³)
        [TestCase(0f, 1.225f)]
        [TestCase(2000f, 1.006f)]
        [TestCase(3900f, 0.828f)]
        public void Density_MatchesIsaReference(float altitude, float expected)
        {
            Assert.AreEqual(expected, Atmosphere.Density(altitude), 0.005f);
        }

        [Test]
        public void Density_DecreasesWithAltitude()
        {
            Assert.Less(Atmosphere.Density(3900f), Atmosphere.Density(2000f));
        }

        [Test]
        public void Density_IsRoughly22PercentHigherAt2000mThanAt3900m()
        {
            float ratio = Atmosphere.Density(2000f) / Atmosphere.Density(3900f);
            Assert.AreEqual(1.216f, ratio, 0.01f);
        }

        [Test]
        public void ColderAirIsDenser()
        {
            Assert.Greater(Atmosphere.Density(3000f, -10f), Atmosphere.Density(3000f, 0f));
        }
    }
}
