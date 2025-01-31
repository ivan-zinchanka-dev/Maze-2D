using VContainer.Unity;

namespace Maze2D.Test
{
    public class GamePresenter : ITickable
    {
        readonly HelloWorldService _helloWorldService;

        public GamePresenter(HelloWorldService helloWorldService)
        {
            _helloWorldService = helloWorldService;
        }

        public void Tick()
        {
            _helloWorldService.Hello();
        }
                
    }
}