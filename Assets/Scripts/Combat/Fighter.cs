using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Combat {

    public class Fighter : MonoBehaviour, IAction {

        [SerializeField] float weaponRange = 2f;
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] float weaponDamage = 20f;

        Transform target;
        Mover mover;
        float timeSinceLastAttack = 0;

        private void Start() {

            mover = GetComponent<Mover>();
        }

        private void Update() {

            timeSinceLastAttack += Time.deltaTime;

            if (target == null) return;

            if (!GetIsInRange()) {

                mover.MoveTo(target.position);
            } else {

                mover.Cancel();
                AttackBehaviour();
            }
        }

        private void AttackBehaviour() {

            if (timeSinceLastAttack >= timeBetweenAttacks) {

                // Will trigger Hit() event
                GetComponent<Animator>().SetTrigger("attack");
                timeSinceLastAttack = 0f;
            }
        }

        // Animation event
        void Hit() {

            Health healthComponent = target.GetComponent<Health>();
            healthComponent.TakeDamage(weaponDamage);
        }

        private bool GetIsInRange() {

            return Vector3.Distance(transform.position, target.position) < weaponRange;
        }

        public void Attack(CombatTarget combatTarget) {

            GetComponent<ActionScheduler>().StartAction(this);
            target = combatTarget.transform;
        }

        public void Cancel() {

            target = null;
        }
    }
}

