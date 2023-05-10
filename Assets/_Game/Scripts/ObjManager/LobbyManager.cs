using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class FoodQueue
{
    public int floor;
    public List<FoodObject> readyFoods;
}
public class LobbyManager : MonoBehaviour
{
    public List<ServingPosition> servingPositions;
    public List<Customer> customers;
    [SerializeField] int waitersAmount;
    public List<Waiter> waiters;
    public List<Transform> waiterPos;
    public List<FreePosition> freePos;
    public int waiterIn0Floor = 0;

    public List<ServingPosition> orderingPositions;
    public List<Transform> elevetors;
    public List<Transform> foodElevetors;
    //public List<FoodObject> readyFoods;
    public List<FoodQueue> foodQueues;

    [SerializeField] List<Table> tables;
    [SerializeField] List<Area> area;

    public void UpdateWaiterSpeed()
    {
        float upgradeSpeed = ProfileManager.Instance.playerData.GetUpgradeSave().GetWaiterUpgradeSpeed();
        if (waiters.Count > 1)
        {
            for (int i = 1; i < waiters.Count; i++)
            {
                waiters[i].UpdateSpeed(upgradeSpeed);
            }
        }
    }
    public void InitWaiters()
    {
        waitersAmount = ProfileManager.Instance.playerData.GetUpgradeSave().GetTotalWaiter();
        //waitersAmount = 1;
        waiterIn0Floor = 0;
        for (int i = 0; i < waitersAmount; i++)
        {
            SpawnNewWaitor();
        }
        UpdateWaiterSpeed();
    }
    int indexSpawn = 0;
    public Waiter SpawnNewWaitor()
    {
        float upgradeSpeed = ProfileManager.Instance.playerData.GetUpgradeSave().GetWaiterUpgradeSpeed();
        Waiter waiter = GameManager.Instance.pooling.GetWaiter();
        waiter.UpdateSpeed(upgradeSpeed);
        if (waiter != null)
        {
            waiter.floor = 1;
            waiter.transform.position = waiterPos[indexSpawn].position;
            waiter.gameObject.SetActive(true);
            waiter.Init();
            if (waiters.Count == 0)
            {
                waiter.EggBroke();
            }
            else waiter.EggMode();
            indexSpawn++;
        }
        return waiter;
    }

    public void AddWaiter(Waiter waiter)
    {
        if (!waiters.Contains(waiter))
        {
            waiters.Add(waiter);
        }
    }

    public void InitTable()
    {
        for (int i = 0; i < tables.Count; i++)
        {
            if (ProfileManager.Instance.playerData.GetUpgradeSave().IsTableUnlocked(i))
            {
                tables[i].gameObject.SetActive(true);
                tables[i].InitTable();
            }
            else
            {
                tables[i].DisableTable();
            }
        }
    }
    public void BuildTable(int id)
    {
        tables[id].TableOnDelivery();
    }
    public void MoveTableOnFollowFloor(int id) {
        tables[id].MoveObject();
    }
    public void BuildArea(int id)
    {
        area[id - 1].AreaOnDelivery();
    }
    public void UpdateCustomerSpeed()
    {
        for (int i = 0; i < customers.Count; i++)
        {
            customers[i].UpdateSpeed();
        }
    }
    int customerAmount;
    public void InitCustomer()
    {
        if (ProfileManager.Instance.playerData.currentLevel > 1)
        {
            if (!ProfileManager.Instance.playerData.GetMenuSave().dataFoodSave.CheckHaveMachineUnlock())
            {
                return;
            }
        }
        customerAmount = ProfileManager.Instance.playerData.GetUpgradeSave().GetTotalCustomer();
        StartCoroutine(SpawnMultipleCustomers(customerAmount));
    }
    public void CallCustomer(int amount = 1)
    {
        if(amount == 1) 
        {
            SpawnCustomer();
        }
        else
        {
            StartCoroutine(SpawnMultipleCustomers(amount));
        }
    }
    IEnumerator SpawnMultipleCustomers(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            SpawnCustomer();
            yield return new WaitForSeconds(3f);
        }
    }
    void SpawnCustomer()
    {
        Customer customer = GameManager.Instance.pooling.GetCustomer();
        if (customer != null)
        {
            customer.OnInit();
            customer.transform.position = GameManager.Instance.GetSpawnPosition().position;
            ServeCustomer(customer);
            if (!customers.Contains(customer))
            {
                customers.Add(customer);
            }
        }
    }
    public void CustomersShowOrder()
    {
        for (int i = 0; i < customers.Count; i++)
        {
            customers[i].CustomerDoneOrder();
        }
    }
    public void ServeCustomer(Customer customer)
    {
        ServingPosition serving = GetFreeSit();
        if(serving != null)
        {
            customer.gameObject.SetActive(true);
            serving.SetCustomer(customer);
            if(serving.floor == 0)
            {
                customer.CallCustomerMove(serving);
            }
            else
            {
                customer.CallCustomerMove(serving, elevetors[0]);
            }
            customer.CustomerStartOrder(() => {
                serving.orderProducts.Clear();
                serving.orderProducts.Add(customer.orderProduct);
                orderingPositions.Add(serving);
                CallQueueCustomer(serving);
            });
        }
        else
        {
            customer.gameObject.SetActive(false);
        }
    }
    public ServingPosition GetFreeSit()
    {
        for(int i = 0; i < servingPositions.Count; i++)
        {
            if(!servingPositions[i].taken)
            {
                return servingPositions[i];
            }
        }
        return null;
    }

    void CallQueueCustomer(ServingPosition serving)
    {
        if(serving.floor == 0)
        {
            if(waiterIn0Floor == 0)
            {
                GameManager.Instance.kitchenManager.CallStaffToTakeOrder(serving);
            }
            else
            {
                CallWaiterToTakeOrder(serving);
            }
        }
        else
        {
            CallWaiterToTakeOrder(serving);
        } 
    }

    void CallWaiterToTakeOrder(ServingPosition serving)
    {
        for (int i = 0; i < waiters.Count; i++)
        {
            if(waiters[i].isFree && waiters[i].floor == serving.floor)
            {
                waiters[i].CallWaiterToTakeOrder(serving);
                return;
            }
        }
    }

    public void CallWaiterToBringFood()
    {
        for (int i = 0; i < waiters.Count; i++)
        {
            if (waiters[i].isFree)
            {
                FoodObject f = GetFoodDeQueue(waiters[i].floor);
                //if(readyFoods.Count > 0)
                //{
                //    waiters[i].CallWaiterToBringFood(readyFoods[0]);
                //    DequeueFood(readyFoods[0]);
                //}
                //else
                //{
                //    break;
                //}
                if(f)
                {
                    waiters[i].CallWaiterToBringFood(f);
                    DequeueFood(f, waiters[i].floor);
                }
            }
        }
    }

    public void CallFreeChefDoWork()
    {
        for(int i = 0; i < orderingPositions.Count; i++)
        {
            if(!orderingPositions[i].ordered)
            {
                if (!orderingPositions[i].ordering)
                {
                    CallQueueCustomer(orderingPositions[i]);
                }
            } 
            else
            {
                for (int j = 0; j < orderingPositions[i].orderProducts[0].orderingAmount; j++)
                {
                    GameManager.Instance.kitchenManager.CallStaffToCook(orderingPositions[i]);
                }
            }
        }
    }

    public Transform GetElevetor(int floor)
    {
        return elevetors[floor];
    }

    public Transform GetFoodElevetor(int floor)
    {
        return foodElevetors[floor];
    }

    public void EnqueueFood(int floor, FoodObject food)
    {
        //readyFoods.Add(food);

        bool hasQueue = false;
        for (int i = 0; i < foodQueues.Count; i++)
        {
            if (foodQueues[i].floor == floor)
            {
                foodQueues[i].readyFoods.Add(food);
                hasQueue = true;
                break;
            }
        }
        if(!hasQueue)
        {
            FoodQueue fq = new FoodQueue();
            fq.floor = floor;
            fq.readyFoods = new List<FoodObject>();
            fq.readyFoods.Add(food);
            foodQueues.Add(fq);
        }

        CallWaiterToBringFood();
    }
    public void DequeueFood(FoodObject food, int floor = 0)
    {
        //readyFoods.Remove(food);

        for (int i = 0; i < foodQueues.Count; i++)
        {
            if (foodQueues[i].floor == floor)
            {
                if (foodQueues[i].readyFoods.Count > 0)
                {
                    foodQueues[i].readyFoods.Remove(food);
                }
            }
        }
    }

    FoodObject GetFoodDeQueue(int floor)
    {
        for (int i = 0; i < foodQueues.Count; i++)
        {
            if (foodQueues[i].floor == floor)
            {
                if(foodQueues[i].readyFoods.Count > 0)
                {
                    return foodQueues[i].readyFoods[0];
                }
            }
        }
        return null;
    }

    public FreePosition GetFreePlace()
    {
        for (int i = 0; i < freePos.Count; i++)
        {
            if (!freePos[i].isTaken)
            {
                freePos[i].isTaken = true;
                return freePos[i];
            }
        }
        return freePos[0];
    }
}
