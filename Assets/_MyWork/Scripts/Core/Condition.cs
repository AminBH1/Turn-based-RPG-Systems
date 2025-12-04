using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public class Condition {

    [SerializeField] private Disjunction[] and;

    public bool Check(IEnumerable<IPredicateEvaluator> predicateEvaluatorList) {
        foreach (Disjunction disjunction in and) {
            if (disjunction.Check(predicateEvaluatorList) == false) {
                return false;
            }
        }
        return true;
    }

    [Serializable]
    public class Disjunction {

        [SerializeField] private Predicate[] or;

        public bool Check(IEnumerable<IPredicateEvaluator> predicateEvaluatorList) {
            if (or.Length == 0) {
                return true;
            }

            foreach (Predicate predicate in or) {
                Debug.Log(predicate);
                if (predicate.Check(predicateEvaluatorList)) {
                    return true;
                }
            }
            return false;
        }
    }

    [Serializable]
    public class Predicate {

        [SerializeField] private string predicate;
        [SerializeField] private string[] parametres;
        [SerializeField] private bool negate;

        public bool Check(IEnumerable<IPredicateEvaluator> predicateEvaluatorList) {

            foreach (IPredicateEvaluator evaluator in predicateEvaluatorList) {
                if (evaluator.Evaluate(predicate, parametres) == null) {
                    continue;
                }

                if (evaluator.Evaluate(predicate, parametres) == false) {
                    return negate;
                }
            }
            return true;
        }
    }
}