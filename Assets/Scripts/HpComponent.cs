using UnityEngine;

public class HpComponent : MonoBehaviour {
    [SerializeField] private int myMaxHp;
    [SerializeField] private int myHp;

    public int MaxHp => myMaxHp;
    public int Hp => myHp;
    public bool DestroyOnDeath { get; set; } = true;

    public void GetHit(int dmg) {
        myHp -= dmg;
        var args = new HitArgs(dmg);
        OnHit?.Invoke(this, args);
        
        if (myHp <= 0) {
            OnDeath?.Invoke(this, args);

            if (DestroyOnDeath) {
                Destroy(gameObject);
            }
        }
    }

    public delegate void HitHandler(HpComponent component, HitArgs hitArgs);

    public class HitArgs {
        public HitArgs(int dmg) {
            Damage = dmg;
        }

        public int Damage {
            get;
            private set;
        }
    }
    
    public event HitHandler OnDeath;
    public event HitHandler OnHit;
}
