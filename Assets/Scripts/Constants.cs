using UnityEngine;

public class Constants
{
    /* RESOURCES */
    public const string RESOURCE_ORDER_CARDS = "orderCards";
    public const string RESOURCE_REPTILIANS_CARDS = "reptiliansCards";

    /* SAVEFILES */
    public const string SAVEFILE_DECK_DATA = "playerDeck.json";
    public const string SAVEFILE_CURRENT_LEVEL = "level.json";

    /* LAYERS */
    public const string LAYER_ON_TOP = "OnTop";

    /* POSITIONS */
    public static readonly Vector3 POS_PLAYER_DECK = new(7.3f, -1f, 0f);
    public static readonly Vector3 POS_REPTILIANS_DECK = new(7.3f, 1f, 0f);
    public const float POS_PLAYER_HAND_Y = -3.6f;
    public const float POS_REPTILIANS_HAND_Y = 3.6f;

    /* MARGINS */
    public const float MARGIN_BETWEEN_CARDS = 1.2f;

    /* TIME */
    public const float TIME_GET_CARD_ANIMATION = 0.2f;
    public const float TIME_DESTROY_CARD_ANIMATION = 0.2f;
    public const float TIME_CARD_ATTACK_ANIMATION = 0.3f;
    public const float TIME_HIGHLIGHT_ANIMATION = 0.5f;
    public const float TIME_FADE_ANIMATION = 0.3f;
    public const float TIME_ROTATE3D_ANIMATION = 0.7f;
    public const float TIME_UFO_ANIMATION = 2f;
    public const float TIME_DELAY = 1f;
    public const int TIME_TURN_DURATION = 20;
    public const float TIME_TEXT_DISPLAY_DURATION = 0.5f;

    /* COLORS */
    public static readonly Color COLOR_RED = new(177f / 255f, 24f / 255f, 32f / 255f);

    /* MAX */
    public const int MAX_CARDS_IN_HAND = 8;
    public const int MAX_SPAWN_POINTS = 10;
}
