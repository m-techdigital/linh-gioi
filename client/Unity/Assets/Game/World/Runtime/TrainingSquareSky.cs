using UnityEngine;
namespace LinhGioi.World
{
    public sealed partial class OnboardingBlockoutWorld
    {
        private Material _squareSky,_previousSquareSky;
        private void CreateSquareSky()
        {
            var shader=Resources.Load<Shader>("LGOCityDaySky");
            if(shader==null || !shader.isSupported)throw new System.InvalidOperationException("City daylight sky unavailable.");
            _previousSquareSky=RenderSettings.skybox;
            _squareSky=new Material(shader){name="Linh Thanh daylight clouds"};
            RenderSettings.skybox=_squareSky;
        }
    }
}
