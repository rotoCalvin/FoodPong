using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FoodManager : MonoBehaviour
{
    public List<GameObject> foods;
    public float foodSpawnTime = 0.5f;
    public int maxFoodItems = 30;

    public bool dropFood { get; set; }
    public float dropLeftX = -5f;
    public float dropRightX = 5f;
    public float dropY = 7f;

    private float lastSpawnTime = 0.0f;
    private int lastFoodIndex = -1;

    private List<GameObject> foodsOnScreen;




    // Use this for initialization
    void Start ()
    {
        foodsOnScreen = new List<GameObject>();
        dropFood = false;
    }


    public void OnDrawGizmosSelected()
    {
        Gizmos.DrawLine(new Vector2(dropLeftX, dropY), new Vector2(dropRightX, dropY));
    }



    // Update is called once per frame
    void Update()
    {
        if (dropFood)
        {
            CreateNewFoodDrop();
        }
    }



    public void CreateNewFoodDrop()
    {
        float updateTime = Time.unscaledTime;
        if (updateTime - lastSpawnTime > foodSpawnTime)
        {
            if (foodsOnScreen.Count >= maxFoodItems)
            {
                GameObject oldestFood = foodsOnScreen[0];
                Food oldestFoodScript = oldestFood.GetComponent<Food>();
                
                foodsOnScreen.RemoveAt(0);

                oldestFoodScript.DestroyEverything();
                Destroy(oldestFood, 1f);
                oldestFoodScript.FadeOut();
            }

            Vector3 dropLocation = new Vector3(Random.Range(dropLeftX, dropRightX), dropY, 0);
            GameObject nextFood = Instantiate(GenerateNextFood(), dropLocation, Quaternion.identity) as GameObject;
            nextFood.GetComponent<Rigidbody2D>().transform.Rotate(RandomRotation());


            //Debug.DrawLine(boundingBox.topLeft, boundingBox.bottomLeft, Color.red);
            //Debug.Log("food item " + foodsOnScreen.Count + " is a " + nextFood.gameObject.name);

            foodsOnScreen.Add(nextFood);

            lastSpawnTime = updateTime;
        }
    }




    public List<GameObject> GetCurrentFoods()
    {
        List<GameObject> nonSplatFoods = new List<GameObject>();
        foreach (GameObject food in foodsOnScreen)
        {
            if (food.GetComponent<Food>().IsAlive())
            {
                nonSplatFoods.Add(food);
            }
        }
        return nonSplatFoods;
    }





    private GameObject GenerateRandomFood()
    {
        return foods[Random.Range(0, foods.Count)];
    }

    private GameObject GenerateNextFood()
    {
        lastFoodIndex++;
        if (lastFoodIndex >= foods.Count) lastFoodIndex = 0;
        return foods[lastFoodIndex];
    }


    public void Setup(int maxFood, float spawnTime)
    {
        maxFoodItems = maxFood;
        foodSpawnTime = spawnTime;
    }



    public static Vector3 RandomRotation()
    {
        float randomRotation = Random.Range(-30f, 30f);
        return new Vector3(0f, 0f, randomRotation);
    }



}
