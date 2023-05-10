using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpecialCargo : MonoBehaviour
{
    [SerializeField] GameObject packContainer;
    public List<UISpecialOffer> uISpecialOffers;
    public UIRevenuePack revenuePack;
    [SerializeField] Scrollbar scrollbar;
    float unitDivide;
    bool stopFlag;
    float newValueMove;
    float autoMoveCoolDown = 3.5f;
    float autoMoveCounter = 0;
    int checkPos = 0;
    [SerializeField] List<Button> navItemBtn;
    [SerializeField] Color navActiveColor;
    [SerializeField] Color navDeactiveColor;
    [SerializeField] RectTransform rt;
    [SerializeField] RectTransform thisRect;

    bool revenuePackRemain;
    int remainPack;
    public void InitOffer(List<OfferData> listOffer, OfferData revenueData)
    {
        LayoutRebuilder.MarkLayoutForRebuild(thisRect);
        revenuePackRemain = revenuePack.Init(revenueData, rt.rect.width);
        remainPack = 0;
        for (int i = 0; i < uISpecialOffers.Count; i++)
        {
            if(uISpecialOffers[i].Init(listOffer[i], rt.rect.width))
            {
                remainPack++;
            }
        }
        //unitDivide = 1f / (uISpecialOffers.Count - 1);
        if(remainPack == 0)
        {
            packContainer.SetActive(false);
        }
        else
        {
            packContainer.SetActive(true);
            unitDivide = 1f / (remainPack - 1);
            for (int i = 0; i < navItemBtn.Count; i++)
            {
                if (i < remainPack)
                {
                    int index = i;
                    navItemBtn[index].onClick.RemoveAllListeners();
                    navItemBtn[index].onClick.AddListener(() => {
                        checkPos = index;
                        newValueMove = unitDivide * checkPos;
                        stopFlag = false;
                        autoMoveCounter = 0;
                        SetButtonColor(index);
                    });
                }
                else
                {
                    navItemBtn[i].gameObject.SetActive(false);
                }
            }
        }

        if (!revenuePackRemain && remainPack == 0)
        {
            gameObject.SetActive(false);
        }
        LayoutRebuilder.ForceRebuildLayoutImmediate(thisRect);
        stopFlag = false;
        newValueMove = 0;
        autoMoveCounter = 0;
        scrollbar.value = 0;
        checkPos = 0;
        SetButtonColor(checkPos);
    }

    void SetButtonColor(int index)
    {
        for (int j = 0; j < navItemBtn.Count; j++)
        {
            if (j == index)
            {
                navItemBtn[j].image.color = navActiveColor;
            }
            else
            {
                navItemBtn[j].image.color = navDeactiveColor;
            }
        }
    }

    public void CheckScroll()
    {
        for (int i = 0; i < uISpecialOffers.Count; i++)
        {
            if (scrollbar.value < ( unitDivide * (i + 0.5)))
            {
                //scrollbar.value = unitDivide * i;
                newValueMove = unitDivide * i;
                checkPos = i;
                SetButtonColor(checkPos);
                break;
            }
        }
        stopFlag = false;
    }

    public void BeginDrag()
    {
        stopFlag = true;
        autoMoveCounter = 0;
    }

    private void Update()
    {
        if(!stopFlag)
        {
            if(scrollbar.value > newValueMove)
            {
                scrollbar.value -= Time.deltaTime;
                if(scrollbar.value <= newValueMove)
                {
                    scrollbar.value = newValueMove;
                    stopFlag = true;
                }
            }
            else
            {
                scrollbar.value += Time.deltaTime;
                if (scrollbar.value >= newValueMove)
                {
                    scrollbar.value = newValueMove;
                    stopFlag = true;
                }
            }
        }
        else
        {
            autoMoveCounter += Time.deltaTime;
            if(autoMoveCounter >= autoMoveCoolDown)
            {
                autoMoveCounter = 0;
                checkPos++;
                if (checkPos == uISpecialOffers.Count) checkPos = 0;
                newValueMove = unitDivide * checkPos;
                stopFlag = false;
                SetButtonColor(checkPos);
            }
        }
    }
}
