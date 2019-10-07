using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class World : MonoBehaviour
{
    [SerializeField]
    private WorldContext worldContext;

    [SerializeField]
    private GameObject defeatScreen;

    [SerializeField]
    private GameObject victoryScreen;

    [SerializeField]
    private float fadeInDuration;

    private ISystem[] systems;

    private GameStateComponent gameStateComponent;

    private bool gameOver = false;

    void Start()
    {
        systems = new ISystem[]
        {
            new LocomotionSystem(),
            new PositionOnTileSystem(),
            new InputSystem(),
            new H0nckerAnimationSystem(),
            new FeetSystem(),
            new BeakMagnetSystem(),
            new AntagonistSystem(),
            new InventorySystem(),
            new TimeSystem(),
            new BreakTossedSystem(),
            new SmackAgainstTheWallSystem(),
            new SharkAISystem(),
            new DoombaAISystem(),
            new TerminatorAISystem(),
            new ProjectileSystem()
            //new SUPERHONKSystem()
        };
        
        for (int i = 0; i < systems.Length; i++)
        {
            systems[i].Cache(worldContext);
        }

        gameStateComponent = worldContext.Get<GameStateComponent>(0);
    }
    
    void Update()
    {
        if (gameStateComponent.GameState != GameState.RUNNING)
        {
            if (!gameOver && gameStateComponent.GameState != GameState.PAUSED)
            {
                var transparent = new Color(0f, 0f, 0f, 0f);

                if (gameStateComponent.GameState == GameState.DEFEAT)
                {
                    foreach (var image in defeatScreen.GetComponentsInChildren<Image>())
                    {
                        var color = image.color;

                        image.color = transparent;

                        image.DOColor(color, fadeInDuration);
                    }

                    foreach (var textComponent in defeatScreen.GetComponentsInChildren<Text>())
                    {
                        var color = textComponent.color;

                        textComponent.color = transparent;

                        textComponent.DOColor(color, fadeInDuration);
                    }

                    defeatScreen.SetActive(true);
                }

                if (gameStateComponent.GameState == GameState.VICTORY)
                {
                    foreach (var image in victoryScreen.GetComponentsInChildren<Image>())
                    {
                        var color = image.color;

                        image.color = transparent;

                        image.DOColor(color, fadeInDuration);
                    }

                    foreach (var textComponent in victoryScreen.GetComponentsInChildren<Text>())
                    {
                        var color = textComponent.color;

                        textComponent.color = transparent;

                        textComponent.DOColor(color, fadeInDuration);
                    }

                    victoryScreen.SetActive(true);
                }

                gameOver = true;
            }

            return;
        }

        for (int i = 0; i < systems.Length; i++)
        {
            systems[i].Handle(worldContext);
        }
    }
}