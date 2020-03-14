using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using RPG.Movement;
using UnityEngine;

namespace RPG.Combat {

    public class Fighter : MonoBehaviour, IAction {

        [SerializeField] float weaponRange = 2f;
        [SerializeField] float timeBetweenAttacks = 1f;

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

                GetComponent<Animator>().SetTrigger("attack");
                timeSinceLastAttack = 0f;
            }
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

        // Animation event
        void Hit() {

            
        }
    }
}

