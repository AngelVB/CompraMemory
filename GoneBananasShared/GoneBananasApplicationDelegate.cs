using CocosDenshion;
using CocosSharp;

namespace GoneBananas
{
    public class GoneBananasApplicationDelegate : CCApplicationDelegate
    {
		public static CCPoint scale;

        public override void ApplicationDidFinishLaunching (CCApplication application, CCWindow mainWindow)
        {
            application.PreferMultiSampling = false;
            
			//Directorio para assets. Se copia en cada una de las plataformas.
			application.ContentRootDirectory = "Content";

            application.ContentSearchPaths.Add("hd");

			//Precarga de efectos de sonido
            CCSimpleAudioEngine.SharedEngine.PreloadEffect ("Sounds/tap");
           
			//Precarga de música de fondo
			CCSimpleAudioEngine.SharedEngine.PreloadBackgroundMusic ("Sounds/backgroundMusic");
           
			//Tamaño de ventana
			CCSize winSize = mainWindow.WindowSizeInPixels;
            
			//Ajustamos resolución de juego
			CCScene.SetDefaultDesignResolution (winSize.Width, winSize.Height, CCSceneResolutionPolicy.ShowAll);


			// Mínima resolución para sacar la escala
			CCSize reference = new CCSize(1900, 1080);

		

			//Calculamos el factor de escala con el que redimendionaremos cada una de nuestras medidas y coordenadas.
			scale = new CCPoint(winSize.Width / reference.Width, winSize.Height / reference.Height);


			//Pantalla (layer) inicial
			CCScene scene = GameStartLayer.GameStartLayerScene(mainWindow,1);

			//Run
			mainWindow.RunWithScene (scene);
        }

		//Controlamos cuando la aplicación pierde foco
        public override void ApplicationDidEnterBackground (CCApplication application)
        {
            // Pausamos las animaciones
            application.Paused = true;

            // Pausamos la música de fondo
            CCSimpleAudioEngine.SharedEngine.PauseBackgroundMusic ();
        }

		//Controlamos cuando la aplicación vuelve
        public override void ApplicationWillEnterForeground (CCApplication application)
        {
			//Activamos las animaciones
            application.Paused = false;

            // Activamos la música de fondo.
            CCSimpleAudioEngine.SharedEngine.ResumeBackgroundMusic ();
        }
    }
}