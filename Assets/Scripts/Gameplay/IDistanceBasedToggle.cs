
namespace Game.Gameplay
{
    public interface IDistanceBasedToggle
    {
        void OnPlayerEnterRange();
        void OnPlayerExitRange();
    }
}
