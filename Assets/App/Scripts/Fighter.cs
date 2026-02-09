using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class Fighter : MonoBehaviour
{
    [InlineEditor] public SO_FighterData data;

    [SerializeField] private TMP_Text NameText;
    [SerializeField] private TMP_Text CritText;
    [SerializeField] private UnityEngine.UI.Slider HealthBar;

    private string _name;
    private int _currentHealth;
    private int _critChance;

    private bool isDefending = false;

    private void Awake()
    {
        _name = data.FighterName;
        _currentHealth = data.MaxHealth;
        _critChance = data.CriticalChance;

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

    public void PerformTurn(Fighter target)
    {
        isDefending = false;

        int actionIndex = Random.Range(0, 3);

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
        _currentHealth += data.HealAmount;

        if (_currentHealth > data.MaxHealth)
        {
            _currentHealth = data.MaxHealth;
        }

        if (HealthBar != null) HealthBar.value = _currentHealth;

        Debug.Log(_name + " se soigne et récupère " + data.HealAmount + " PV");
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
}