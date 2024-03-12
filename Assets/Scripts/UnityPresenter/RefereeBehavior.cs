using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace LostCities.UnityPresenter
{
    public class RefereeBehavior : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI gameEndText;
        [SerializeField] TextMeshProUGUI playerScoreText;
        [SerializeField] TextMeshProUGUI opponentScoreText;
        [SerializeField] Image placementReminderPanel;
        [SerializeField] Image refillReminderPanel;

        public GameModel.LocalPlayer LocalPlayerModel => referee.LocalPlayer;
        public GameModel.VirtualPlayer OpponentPlayerModel => referee.OpponentPlayer;

        public DrawPileBehavior CommonDrawPileObj { get; private set; }
        public DiscardPileBehavior[] CommonDiscardPileObjs { get; private set; }
        public PlayerBehavior LocalPlayerObj { get; private set; }
        public VirtualPlayerBehavior OpponentPlayerObj { get; private set; }

        private void Awake()
        {
            Application.targetFrameRate = 60;

            // Bind behavior objects
            CommonDrawPileObj = GetComponentInChildren<DrawPileBehavior>(includeInactive: true);
            CommonDiscardPileObjs = GetComponentsInChildren<DiscardPileBehavior>(includeInactive: true);
            LocalPlayerObj = FindObjectOfType<LocalPlayerBehavior>(includeInactive: true);
            OpponentPlayerObj = FindObjectOfType<VirtualPlayerBehavior>(includeInactive: true);

            referee.OnReset += RefereeModel_OnReset;
            referee.OnStartGame += RefereeModel_OnStartGame;
            referee.OnTurnStarted += RefereeModel_OnTurnStarted;
            referee.OnCardSelected += RefereeModel_OnCardSelected;
            referee.OnIllegalMove += RefereeModel_OnIllegalMove;
            referee.OnCardPlaced += RefereeModel_OnCardPlaced;
            referee.OnHandRefilled += RefereeModel_OnHandRefilled;
            referee.OnTurnEnded += RefereeModel_OnTurnEnded;
            referee.OnGameOver += RefereeModel_OnGameOver;

            playerObjs[0] = LocalPlayerObj;
            playerObjs[1] = OpponentPlayerObj;

            scoreTexts[0] = playerScoreText;
            scoreTexts[1] = opponentScoreText;
        }

        private void Start()
        {
            gameEndText.gameObject.SetActive(false);
            placementReminderPanel.gameObject.SetActive(false);
            refillReminderPanel.gameObject.SetActive(false);
            referee.Reset();
            UpdatePlayerScoreText(0);
            UpdatePlayerScoreText(1);
            referee.StartGame(); // Deal cards to each player and triggers OnStartGame when ready to start the game loop
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        private void UpdatePlayerScoreText(int playerIndex)
        {
            int totalScore = referee.CurrentScore[playerIndex];
            scoreTexts[playerIndex].text = $"{playerObjs[playerIndex].name} Score: {totalScore}";
        }

        private void RefereeModel_OnReset()
        {
            // Bind behavior objects to their new models
            // TODO - find a better way to bind these
            CommonDrawPileObj.CardPile = referee.CommonDrawPile;
            LocalPlayerObj.PlayerModel = referee.LocalPlayer;
            LocalPlayerObj.HandObj.Hand = referee.LocalPlayer.Hand;
            OpponentPlayerObj.PlayerModel = referee.OpponentPlayer;
            OpponentPlayerObj.HandObj.Hand = referee.OpponentPlayer.Hand;

            for (int e = 0; e < (int)GameModel.Expedition.COUNT; e++)
            {
                CommonDiscardPileObjs[e].CardPile = referee.CommonDiscardPiles[e];
                CommonDiscardPileObjs[e].name = $"Discard Pile ({(GameModel.Expedition)e})";
                LocalPlayerObj.ExpeditionPileObjs[e].ExpeditionPile = referee.LocalPlayer.ExpeditionPiles[e];
                OpponentPlayerObj.ExpeditionPileObjs[e].ExpeditionPile = referee.OpponentPlayer.ExpeditionPiles[e];
            }
        }
        
        private void RefereeModel_OnStartGame()
        {
            StartCoroutine(referee.DoGameLoop());
        }

        private void RefereeModel_OnTurnStarted(GameModel.Player player)
        {
            Debug.Log($"{playerObjs[player.TurnOrder].name} has STARTED their turn");
            placementReminderPanel.gameObject.SetActive(player == LocalPlayerModel);
        }

        private void RefereeModel_OnCardSelected(GameModel.Player player)
        {
            Debug.Log($"{playerObjs[player.TurnOrder].name} SELECTED {player.SelectedCard.Label}");
        }

        private void RefereeModel_OnIllegalMove(GameModel.Player player, GameModel.IllegalMove illegalMoveType)
        {
            Debug.Log($"{playerObjs[player.TurnOrder].name} {illegalMoveType}");
        }
        private void RefereeModel_OnCardPlaced(GameModel.Player player)
        {
            Debug.Log($"{playerObjs[player.TurnOrder].name} PLACED {player.SelectedCard.Label}");
            CommonDrawPileObj.IsPlayerSelectable = true;
            foreach (var discardPileObj in CommonDiscardPileObjs)
            {
                bool isSelectable = player.CardAction == GameModel.CardAction.PLAY ||
                    player.SelectedCard.Expedition != discardPileObj.Expedition;
                discardPileObj.IsPlayerSelectable = isSelectable;
            }
            placementReminderPanel.gameObject.SetActive(false);
            refillReminderPanel.gameObject.SetActive(player == LocalPlayerModel);
        }

        private void RefereeModel_OnHandRefilled(GameModel.Player player)
        {
            Debug.Log($"{playerObjs[player.TurnOrder].name} DREW {player.MostRecentCardDraw.card.Label} from the {player.MostRecentCardDraw.pile.Label}");
            refillReminderPanel.gameObject.SetActive(false);
            UpdatePlayerScoreText(player.TurnOrder);
        }

        private void RefereeModel_OnTurnEnded(GameModel.Player player)
        {
            // TODO - FIXME! Disabling selectables should occur in RefereeModel_OnHandRefilled,
            // but the timing prevents the selected pile's "pop-up" animation from finishing.
            CommonDrawPileObj.IsPlayerSelectable = false;
            foreach (var discardPile in CommonDiscardPileObjs)
            {
                discardPile.IsPlayerSelectable = false;
            }

            Debug.Log($"{playerObjs[player.TurnOrder].name} has FINISHED their turn");
        }

        private void RefereeModel_OnGameOver()
        {            
            StartCoroutine(DisplayGameOver());
        }     

        private IEnumerator DisplayGameOver()
        {
            Debug.Log("GAME OVER");
            gameEndText.gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(5f);
            // TODO - present retry and quit options
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private readonly GameModel.Referee referee = new();
        private readonly TextMeshProUGUI[] scoreTexts = new TextMeshProUGUI[2];
        private readonly PlayerBehavior[] playerObjs = new PlayerBehavior[2];
    }
}
