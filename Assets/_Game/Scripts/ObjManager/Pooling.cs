using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pooling : MonoBehaviour
{
    [Header("============Staff===============")]
    [SerializeField] Staff staffPrefab;
    [SerializeField] Transform staffContainer;
    [SerializeField] List<Staff> staffs;
    [SerializeField] int staffAmount = 5;
    [SerializeField] Staff inviteStaffPrefab;

    void PoolStaff()
    {
        for(int i = 0; i < staffAmount; i++)
        {
            SpawnStaff();
        }
    }
    Staff SpawnStaff()
    {
        Staff staff = Instantiate(staffPrefab, staffContainer);
        staffs.Add(staff);
        staff.gameObject.SetActive(false);
        return staff;
    }
    public Staff GetStaff()
    {
        for(int i = 0; i < staffs.Count; i++)
        {
            if(!staffs[i].gameObject.activeSelf)
            {
                Staff toGet = staffs[i];
                staffs.Remove(toGet);
                return toGet;
            }
        }
        Staff staff = Instantiate(staffPrefab, staffContainer);
        staff.gameObject.SetActive(false);
        //staffs.Add(staff);
        return staff;
    }


    [Header("==========Waiter==============")]
    [SerializeField] Waiter waiterPrefab;
    [SerializeField] Transform waiterContainer;
    [SerializeField] List<Waiter> waiters;
    [SerializeField] int waiterAmount = 2;

    void PoolWaiter()
    {
        for (int i = 0; i < waiterAmount; i++)
        {
            SpawnWaiter();
        }
    }
    Waiter SpawnWaiter()
    {
        Waiter waiter = Instantiate(waiterPrefab, waiterContainer);
        waiters.Add(waiter);
        waiter.gameObject.SetActive(false);
        return waiter;
    }
    public Waiter GetWaiter()
    {
        for (int i = 0; i < waiters.Count; i++)
        {
            if (!waiters[i].gameObject.activeSelf)
            {
                return waiters[i];
            }
        }
        Waiter waiter = Instantiate(waiterPrefab, waiterContainer);
        waiter.gameObject.SetActive(false);
        waiters.Add(waiter);
        return waiter;
    }
    
    [Header("================Customer=================")]
    [SerializeField] Customer customerPrefab;
    [SerializeField] Transform customerContainer;
    [SerializeField] List<Customer> customers;
    [SerializeField] int customerAmount = 5;
    int customerIndex;
    void PoolCustomer()
    {
        for (int i = 0; i < customerAmount; i++)
        {
            SpawnCustomer();
        }
    }
    Customer SpawnCustomer()
    {
        Customer customer = Instantiate(customerPrefab, customerContainer);
        customers.Add(customer);
        customer.transform.position = GameManager.Instance.GetSpawnPosition().position;
        customer.gameObject.SetActive(false);
        return customer;
    }
    public Customer GetCustomer()
    {
        for (int i = 0; i < customers.Count; i++)
        {
            if (!customers[i].gameObject.activeSelf)
            {
                return customers[i];
            }
        }
        Customer customer = Instantiate(customerPrefab, customerContainer);
        customer.transform.position = GameManager.Instance.GetSpawnPosition().position;
        customer.gameObject.SetActive(false);
        customers.Add(customer);
        return customer;
    }

    [Header("===================FoodObject======================")]
    [SerializeField] FoodObject foodPrefab;
    [SerializeField] Transform foodObjContainer;
    [SerializeField] List<FoodObject> foodObjects;
    [SerializeField] int foodAmount = 3;
    List<FoodDataConfig> listFoodConfig;
    void PoolListFood()
    {
        listFoodConfig = ProfileManager.Instance.dataConfig.GetMenuDataByLevel().listFoodConfig;
        for (int j = 0; j < foodAmount; j++)
        {
            SpawnNewFood();
        }
    }

    public FoodObject SpawnNewFood()
    {
        FoodObject food = Instantiate(foodPrefab, foodObjContainer);
        foodObjects.Add(food);
        food.gameObject.SetActive(false);
        return food;
    }

    [SerializeField] Mesh defaultMesh;
    Mesh GetFoodMeshById(FoodID foodID)
    {
        for (int i = 0; i < listFoodConfig.Count; i++)
        {
            if(listFoodConfig[i].foodID == foodID)
            {
                return listFoodConfig[i].meshFood;
            }
        }
        return defaultMesh;
    }

    public FoodObject GetFoodByFoodID(FoodID foodID)
    {
        for (int i = 0; i < foodObjects.Count; i++)
        {
            if(!foodObjects[i].gameObject.activeSelf)
            {
                foodObjects[i].InitFood(foodID, GetFoodMeshById(foodID));
                return foodObjects[i];
            }
            FoodObject food = SpawnNewFood();
            food.InitFood(foodID, GetFoodMeshById(foodID));
            return food;
        }
        return null;
    }

    public void ReturnFood(FoodObject food)
    {
        if( food!= null)
        {
            food.gameObject.SetActive(false);
            food.transform.parent = foodObjContainer;
        }
    }

    [Header("=====================Coin Effect========================")]
    [SerializeField] ParticleSystem coinEffectPrefab;
    [SerializeField] Transform coinEffectContainer;
    [SerializeField] List<ParticleSystem> coinEffects;
    [SerializeField] int coinEffectAmount = 5;

    void PoolCoinEffect()
    {
        for (int i = 0; i < coinEffectAmount; i++)
        {
            SpawnCoinEffect();
        }
    }
    ParticleSystem SpawnCoinEffect()
    {
        ParticleSystem coin = Instantiate(coinEffectPrefab, coinEffectContainer);

        coinEffects.Add(coin);
        //coin.gameObject.SetActive(false);
        return coin;
    }
    [SerializeField] int coinBurst = 5;
    public ParticleSystem GetCoinEffect(Vector3 position)
    {
        for (int i = 0; i < coinEffects.Count; i++)
        {
            //if (!coinEffects[i].activeSelf)
            if (!coinEffects[i].isPlaying)
            {
                coinEffects[i].Emit(coinBurst);
                coinEffects[i].transform.position = position;
                return coinEffects[i];
            }
        }
        ParticleSystem coin = Instantiate(coinEffectPrefab, coinEffectContainer);
        coin.Emit(coinBurst);
        coin.transform.position = position;
        coinEffects.Add(coin);
        return coin;
    }

    [Header("========================Coin Pref========================")]
    [SerializeField] TipCoin coinPrefab;
    [SerializeField] Transform coinContainer;
    [SerializeField] List<TipCoin> tipCoins;
    [SerializeField] int coinAmount = 5;

    void PoolCoin()
    {
        for (int i = 0; i < coinAmount; i++)
        {
            SpawnCoin();
        }
    }
    TipCoin SpawnCoin()
    {
        TipCoin coin = Instantiate(coinPrefab, coinContainer);
        tipCoins.Add(coin);
        coin.gameObject.SetActive(false);
        return coin;
    }
    public TipCoin GetCoin(Vector3 position)
    {
        for (int i = 0; i < tipCoins.Count; i++)
        {
            if (!tipCoins[i].gameObject.activeSelf)
            {
                tipCoins[i].gameObject.SetActive(true);
                tipCoins[i].transform.position = position;
                return tipCoins[i];
            }
        }
        TipCoin coin = Instantiate(coinPrefab, coinContainer);
        coin.transform.position = position;
        tipCoins.Add(coin);
        return coin;
    }

    public void FreeCoin(TipCoin t)
    {
        t.gameObject.SetActive(false);
        t.transform.parent = coinContainer;
    }

    [Header("=====================Value Change Effect========================")]
    [SerializeField] UIValueChangeMove valueChangeMove;
    [SerializeField] Transform valueChangeMoveParent;
    [SerializeField] List<UIValueChangeMove> valueChangeMoves;
    [SerializeField] int countValueChange = 5;

    void PoolValueChangeEffect()
    {
        for (int i = 0; i < coinAmount; i++)
        {
            SpawnValueChangeEffect();
        }
    }

    UIValueChangeMove SpawnValueChangeEffect()
    {
        UIValueChangeMove poolObject = Instantiate(valueChangeMove, valueChangeMoveParent);
        valueChangeMoves.Add(poolObject);
        poolObject.gameObject.SetActive(false);
        return poolObject;
    }

    public UIValueChangeMove GetValueChangeEffect(Vector3 position)
    {
        for (int i = 0; i < valueChangeMoves.Count; i++)
        {
            if (!valueChangeMoves[i].gameObject.activeSelf)
            {
                valueChangeMoves[i].gameObject.SetActive(true);
                valueChangeMoves[i].transform.position = position;
                return valueChangeMoves[i];
            }
        }
        UIValueChangeMove poolObject = Instantiate(valueChangeMove, valueChangeMoveParent);
        poolObject.transform.position = position;
        valueChangeMoves.Add(poolObject);
        return poolObject;
    }

    void Start()
    {
        PoolStaff();
        PoolWaiter();
        PoolCustomer();
        PoolCoinEffect();
        PoolCoin();
        PoolListFood(); 
        PoolValueChangeEffect();
    }

}
