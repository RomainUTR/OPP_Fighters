using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    [InlineEditor] public SO_FighterData data;

    [SerializeField] private TMP_Text NameText;
    [SerializeField] private TMP_Text CritText;
    [SerializeField] private UnityEngine.UI.Slider HealthBar;

    public int potions = 0;

    private string _name;
    private int _currentHealth;
    private int _critChance;

    private bool isDefending = false;
    public bool isPlayer = false;

    private void Awake()
    {
        Init(data);

        if (isPlayer)
        {
            potions = 5;
        }
    }

    public void Init(SO_FighterData newData)
    {
        data = newData;

        _name = data.FighterName;
        _currentHealth = data.MaxHealth;
        _critChance = data.CriticalChance;
        isDefending = false;

        if (HealthBar != null)
        {
            HealthBar.maxValue = data.MaxHealth;
            HealthBar.value = _currentHealth;
        }

        if (NameText != null)
        {
            NameText.text = _name;
        }
        if (CritText != null)
        {
            CritText.text = "Crit: " + _critChance + "%";
        }
    }

    public void PerformAITurn(Fighter target)
    {
        int randomChoice = Random.Range(0, 3);
        ExecuteAction(randomChoice, target);
    }

    public void ExecuteAction(int actionIndex, Fighter target)
    {
        isDefending = false;
        if (NameText != null) NameText.color = Color.white;

        switch (actionIndex)
        {
            case 0:
                Attack(target);
                break;
            case 1:
                Heal();
                break;
            case 2:
                Defend();
                break;
        }
    }

    public void Attack(Fighter target)
    {
        int damage = Random.Range(data.MinDamage, data.MaxDamage);

        int randomValue = Random.Range(0, 101); 
        if (randomValue < _critChance)
        {
            damage *= 2;
            Debug.Log(_name + " inflige un COUP CRITIQUE !");
        }
        int realDamageTaken = target.TakeDamage(damage);

        Debug.Log(_name + " attaque " + target.GetName() + " et inflige " + realDamageTaken + " dégâts.");
    }

    public int TakeDamage(int amount)
    {
        if (isDefending)
        {
            amount /= 2;
            Debug.Log("Défense réussie ! Dégâts réduits.");
        }

        _currentHealth -= amount;
        
        if (_currentHealth < 0)
        {
            _currentHealth = 0;
        }

        if (HealthBar != null)
        {
            HealthBar.value = _currentHealth; 
        }

        return amount;
    }

    private void Heal()
    {
        if (potions > 0)
        {
            potions--;
            _currentHealth += data.HealAmount;
            if (_currentHealth > data.MaxHealth) _currentHealth = data.MaxHealth;
            if (HealthBar != null) HealthBar.value = _currentHealth;
            Debug.Log($"{_name} utilise une potion ! (Reste: {potions})");
        } else
        {
            Debug.Log($"{_name} cherche une potion... mais n'en a plus !");
        }
    }

    private void Defend()
    {
        isDefending = true;
        Debug.Log(_name + " se met en posture défensive !");
    }

    public string GetName()
    {
        return _name;
    }

    public bool IsAlive()
    {
        return _currentHealth > 0;
    }

    public void AddPotion()
    {
        potions++;
        Debug.Log($"{_name} ramasse une potion ! (Total: {potions})");
    }
}