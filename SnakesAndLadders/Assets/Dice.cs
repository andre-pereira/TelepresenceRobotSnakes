using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Database;

public class Dice : MonoBehaviour {
    private Sprite[] diceSides;
    private SpriteRenderer rend;
    public static int whosTurn = 1;
    private bool coroutineAllowed = true;
    public static DatabaseReference reference;
    
    // Use this for initialization
    private void Start () {
        rend = GetComponent<SpriteRenderer>();
        diceSides = Resources.LoadAll<Sprite>("DiceSides/");
        rend.sprite = diceSides[5];
        reference = FirebaseDatabase.DefaultInstance.RootReference;
        saveTrigger();
        FirebaseDatabase.DefaultInstance
        .GetReference("Trigger")
        .ValueChanged += HandleValueChanged;
    }

    void HandleValueChanged(object sender, ValueChangedEventArgs args)
    {
        if (args.DatabaseError != null)
        {
            Debug.LogError(args.DatabaseError.Message);
            return;
        }
        
        if (args.Snapshot.Child("Pressed").Value.ToString() == "true" && coroutineAllowed && whosTurn == -1)
        {   
            saveTrigger();
            StartCoroutine("RollTheDice");
        }
        else
        {
            saveTrigger();
        }
    }

    private void OnMouseDown()
    {
        if (!GameControl.gameOver && coroutineAllowed && whosTurn == 1)
            StartCoroutine("RollTheDice");
    }

    private IEnumerator RollTheDice()
    {
        coroutineAllowed = false;
        int randomDiceSide = 0;
        for (int i = 0; i <= 15; i++)
        {
            randomDiceSide = Random.Range(0, 6);
            rend.sprite = diceSides[randomDiceSide];
            yield return new WaitForSeconds(0.05f);
        }

        GameControl.diceSideThrown = randomDiceSide + 1;
        GameControl.saveGameState(true);
      
        if (whosTurn == 1)
        {
            GameControl.MovePlayer(1);
        } else if (whosTurn == -1)
        {
            GameControl.MovePlayer(2);
        }
        whosTurn *= -1;
        coroutineAllowed = true;
    }
    public static void saveTrigger()
    {
        reference.Child("Trigger").Child("Pressed").SetValueAsync("false");
    }
}
