
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

public class PetPurchase
{
    public GameState gameState;
    
    public PetPurchase(GameState state)
    {
        gameState = state;
    }

    [JSInvokable]
    public void PurchasePet()
    {
        gameState.GetPlayer().Pets.Add(gameState.petToBuy);
        gameState.UpdateState();
    }
    [JSInvokable]
    public void CancelPurchase()
    {
        gameState.petToBuy = null;
        gameState.UpdateState();
    }
    [JSInvokable]
    public void RestorePurchases(string identifier)
    {
        gameState.RestorePet(identifier);
        gameState.UpdateState();
    }
    [JSInvokable]
    public void RefreshUI()
    {
        gameState.GetKongregateLogin();
        gameState.UpdateState();
    }
}
