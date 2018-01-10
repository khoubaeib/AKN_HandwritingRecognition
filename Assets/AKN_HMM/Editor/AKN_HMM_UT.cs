using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using AKeNe.AI.HiddenMarkovModel;
using UnityEngine;

namespace AKeNe.AI.HiddenMarkovModel
{
	[TestFixture]
    class AKN_HMM_UT
    {
		//Markov Model
		
        [Test]
        public void AKN_MM_Creation_UT() 
        {

            /*
            *      0,7                      0,6
            *      ┌──┐                    ┌──┐
            *	   │  │                    │  │
            *      │  ▼        0,4         │  ▼
            *    ┌───────┐<─────────────┌────────┐
            *	 │ Pluie │              │ Soleil │
            *    └───────┘─────────────>└────────┘
            *        ▲         0,3          ▲
            *        │                      │
            *		 │                      │
            *       0,6                    0,4
            */

            /*
             * Création d'un modéle de Markov à deux états 
            */
            AKN_MarkovModel m_MarkovModel = new AKN_MarkovModel(2);

            /*
             * Affectation des états au Modéle(Affectations des probabilitées de démarrage et des noms)
             * 3 méthodes possbiles
            */

            // 1 (les noms sont facultatif)
            m_MarkovModel.MSetStates(new float[] { 0.6F, 0.4F });
            // ou 
            m_MarkovModel.MSetStates(new float[] { 0.6F, 0.4F }, new string[] { "Pluie", "Soleil" });

            // 2 (les noms sont facultatif)
            m_MarkovModel[0].m_startProbability = 0.6F;
            m_MarkovModel[0].m_name = "Pluie";
            m_MarkovModel[1].m_startProbability = 0.4F;
            m_MarkovModel[1].m_name = "Soleil";

            // 3 (Affectations des noms obligatoire au préalable)
            m_MarkovModel["Pluie"].m_startProbability = 0.6F;
            m_MarkovModel["Soleil"].m_startProbability = 0.4F;

            /*
             * Affectation des probabilités de transition au modéle.
             * 3 méthodes possibles sont proposés
            */

            //1
            m_MarkovModel.MSetTransitions(new float[,] { { 0.7F, 0.3F }, { 0.4F, 0.6F } });

            // 2
            m_MarkovModel[0, 0] = 0.7F;
            m_MarkovModel[0, 1] = 0.3F;
            m_MarkovModel[1, 0] = 0.4F;
            m_MarkovModel[1, 1] = 0.6F;

            // 3 (Affectation des noms des états obligatoire au préalable)
            m_MarkovModel["Pluie", "Pluie"] = 0.7F;
            m_MarkovModel["Pluie", "Soleil"] = 0.3F;
            m_MarkovModel["Soleil", "Pluie"] = 0.4F;
            m_MarkovModel["Soleil", "Soleil"] = 0.6F;

            // Testing Values
            Assert.That(m_MarkovModel[0].m_startProbability == 0.6F, "Problem !");
            Assert.That(m_MarkovModel[1].m_startProbability == 0.4F, "Problem !");

            Assert.That(m_MarkovModel[0, 0] == 0.7F, "Problem !");
            Assert.That(m_MarkovModel[0, 1] == 0.3F, "Problem !");
            Assert.That(m_MarkovModel[1, 0] == 0.4F, "Problem !");
            Assert.That(m_MarkovModel[1, 1] == 0.6F, "Problem !");
        }
		
		//Hidden Markov Model

        [Test]
        public void AKN_HMM_Creation_UT()
        {

            //
            //           ┌───────┐                ┌────────┐
            //       0,2 │ Sport │                │  Sport │ 0,5
            //		 0,1 │ Marche│                │ Marche │ 0,3
            //		 0,7 │ Etude │       0,4      │  Etude │ 0,2
            //		    ┌┴───────┴┐<─────────────┌┴────────┴┐
            //		┌───│  Pluie  │              │  Soleil  │───┐
            //	0,7	└──>└─────────┘─────────────>└──────────┘<──┘ 0,6
            //		         ▲           0,3           ▲
            //		         │                         │
            //		         │                         │
            //		        0,6                       0,4
            //

            //Création d'un modéle de markov caché avec deux états cachées et 3 observation
            AKN_HiddenMarkovModel<String> m_HiddenMarkovModel_1 = new AKN_HiddenMarkovModel<String>(2, 3);


            //Affectation des probabilité de démarrage aux états
            //Les noms des états sont facultatifs
            m_HiddenMarkovModel_1.MSetStates(new float[] { 0.6F, 0.4F }, new string[] { "Pluie", "Soleil" });

            //Affectation des probabilités aux transitions
            //Une transition non affecté vaut zéro.
            m_HiddenMarkovModel_1["Pluie", "Pluie"] = 0.7F;
            m_HiddenMarkovModel_1["Pluie", "Soleil"] = 0.3F;
            m_HiddenMarkovModel_1["Soleil", "Pluie"] = 0.4F;
            m_HiddenMarkovModel_1["Soleil", "Soleil"] = 0.6F;

            //	Affection des nom aux états observables
            //	Cette étape et facultatifs
            m_HiddenMarkovModel_1.MSetObservations(new String[] { "Sport", "Marche", "Etude" });

            //Affectation des probabilités des états observables
            // 1(Cette options necessite l'affectation des noms aux observations et aux états)
            m_HiddenMarkovModel_1.MSetEmission("Pluie", "Sport", 0.2F);
            m_HiddenMarkovModel_1.MSetEmission("Pluie", "Marche", 0.1F);
            m_HiddenMarkovModel_1.MSetEmission("Pluie", "Etude", 0.7F);
            m_HiddenMarkovModel_1.MSetEmission("Soleil", "Sport", 0.5F);
            m_HiddenMarkovModel_1.MSetEmission("Soleil", "Marche", 0.3F);
            m_HiddenMarkovModel_1.MSetEmission("Soleil", "Etude", 0.2F);

            // 2
            m_HiddenMarkovModel_1.MSetEmissionByIndex(0, 0, 0.2F);
            m_HiddenMarkovModel_1.MSetEmissionByIndex(0, 1, 0.1F);
            m_HiddenMarkovModel_1.MSetEmissionByIndex(0, 2, 0.7F);
            m_HiddenMarkovModel_1.MSetEmissionByIndex(1, 0, 0.5F);
            m_HiddenMarkovModel_1.MSetEmissionByIndex(1, 1, 0.3F);
            m_HiddenMarkovModel_1.MSetEmissionByIndex(1, 2, 0.2F);

            Assert.That(m_HiddenMarkovModel_1.MGetEmissionByIndex(0, 0) == 0.2F, "Problem !");
            Assert.That(m_HiddenMarkovModel_1.MGetEmissionByIndex(0, 1) == 0.1F, "Problem !");
            Assert.That(m_HiddenMarkovModel_1.MGetEmissionByIndex(0, 2) == 0.7F, "Problem !");
            Assert.That(m_HiddenMarkovModel_1.MGetEmissionByIndex(1, 0) == 0.5F, "Problem !");
            Assert.That(m_HiddenMarkovModel_1.MGetEmissionByIndex(1, 1) == 0.3F, "Problem !");
            Assert.That(m_HiddenMarkovModel_1.MGetEmissionByIndex(1, 2) == 0.2F, "Problem !");

            Assert.That(m_HiddenMarkovModel_1.MGetEmission("Pluie", "Sport") == 0.2F, "Problem !");
            Assert.That(m_HiddenMarkovModel_1.MGetEmission("Pluie", "Marche") == 0.1F, "Problem !");
            Assert.That(m_HiddenMarkovModel_1.MGetEmission("Pluie", "Etude") == 0.7F, "Problem !");
            Assert.That(m_HiddenMarkovModel_1.MGetEmission("Soleil", "Sport") == 0.5F, "Problem !");
            Assert.That(m_HiddenMarkovModel_1.MGetEmission("Soleil", "Marche") == 0.3F, "Problem !");
            Assert.That(m_HiddenMarkovModel_1.MGetEmission("Soleil", "Etude") == 0.2F, "Problem !");

            AKN_HiddenMarkovModel<String> m_HiddenMarkovModel_2 = new AKN_HiddenMarkovModel<String>(2, 2);

            m_HiddenMarkovModel_2.MSetStates(new float[] { 1.0F, 0.0F }, new string[] { "Samedi", "Dimanche" });
            m_HiddenMarkovModel_2.MSetTransitions(new float[,] { { 0.5F, 0.5F }, { 0.0F, 1.0F } });
            m_HiddenMarkovModel_2.MSetObservations(new String[] { "Soleil", "Pluie" });
            m_HiddenMarkovModel_2.MSetEmissions(new float[,] { { 0.3F, 0.7F }, { 0.8F, 0.2F } });

            Assert.That(m_HiddenMarkovModel_2["Samedi"].m_startProbability == 1.0F, "Problem !");
            Assert.That(m_HiddenMarkovModel_2["Dimanche"].m_startProbability == 0.0F, "Problem !");

            Assert.That(m_HiddenMarkovModel_2["Samedi", "Samedi"] == 0.5F, "Problem !");
            Assert.That(m_HiddenMarkovModel_2["Samedi", "Dimanche"] == 0.5F, "Problem !");
            Assert.That(m_HiddenMarkovModel_2["Dimanche", "Samedi"] == 0.0F, "Problem !");
            Assert.That(m_HiddenMarkovModel_2["Dimanche", "Dimanche"] == 1.0F, "Problem !");

            Assert.That(m_HiddenMarkovModel_2.MGetEmission("Samedi", "Soleil") == 0.3F, "Problem !");
            Assert.That(m_HiddenMarkovModel_2.MGetEmission("Samedi", "Pluie") == 0.7F, "Problem !");
            Assert.That(m_HiddenMarkovModel_2.MGetEmission("Dimanche", "Soleil") == 0.8F, "Problem !");
            Assert.That(m_HiddenMarkovModel_2.MGetEmission("Dimanche", "Pluie") == 0.2F, "Problem !");
        }

        [Test]
        public void AKN_HMM_Evaluation_UT() 
        {
            AKN_HiddenMarkovModel<String> m_HiddenMarkovModel;
            String[] observations;
            float result;

            // Exemple 1
            m_HiddenMarkovModel = new AKN_HiddenMarkovModel<String>(2, 2);
            m_HiddenMarkovModel.MSetStates(new float[] { 1.0F, 0.0F }, new string[] { "Samedi", "Dimanche" });
            m_HiddenMarkovModel.MSetTransitions(new float[,] { { 0.5F, 0.5F }, { 0.0F, 1.0F } });
            m_HiddenMarkovModel.MSetObservations(new String[] { "Soleil", "Pluie" });
            m_HiddenMarkovModel.MSetEmissions(new float[,] { { 0.3F, 0.7F }, { 0.8F, 0.2F } });

            observations = new String[] { "Soleil", "Pluie", "Pluie", "Soleil" };

            result = m_HiddenMarkovModel.MEvaluate(observations, eUseAlgorithme.kForward);
            Assert.That(NearlyEqual(result, 0.0334125F), "Problem ! result = " + result);

            result = m_HiddenMarkovModel.MEvaluate(observations, eUseAlgorithme.kBackward);
            Assert.That(NearlyEqual(result, 0.0334125F), "Problem ! result = " + result);

            result = m_HiddenMarkovModel.MEvaluate(observations, eUseAlgorithme.kForwardAndBackward, 0);
            Assert.That(NearlyEqual(result, 0.0334125F), "Problem ! result = " + result);
            result = m_HiddenMarkovModel.MEvaluate(observations, eUseAlgorithme.kForwardAndBackward, 1);
            Assert.That(NearlyEqual(result, 0.0334125F), "Problem ! result = " + result);
            result = m_HiddenMarkovModel.MEvaluate(observations, eUseAlgorithme.kForwardAndBackward, 2);
            Assert.That(NearlyEqual(result, 0.0334125F), "Problem ! result = " + result);
            result = m_HiddenMarkovModel.MEvaluate(observations, eUseAlgorithme.kForwardAndBackward, 3);
            Assert.That(NearlyEqual(result, 0.0334125F), "Problem ! result = " + result);

            // Exemple 2
            m_HiddenMarkovModel = new AKN_HiddenMarkovModel<String>(3, 2);
            m_HiddenMarkovModel.MSetStates(new float[] { 0.5F, 0.3F, 0.2F }, new string[] { "Etat_1", "Etat_2", "Etat_3" });
            m_HiddenMarkovModel.MSetTransitions(new float[,] { { 0.45F, 0.35F, 0.2F }, { 0.1F, 0.5F, 0.4F }, { 0.15F, 0.25F, 0.6F } });
            m_HiddenMarkovModel.MSetObservations(new String[] { "Pile", "Face" });
            m_HiddenMarkovModel.MSetEmissions(new float[,] { { 1.0F, 0.0F }, { 0.5F, 0.5F }, { 0.0F, 1.0F } });

            observations = new String[] { "Pile", "Face", "Face", "Pile", "Pile" };

            result = m_HiddenMarkovModel.MEvaluate(observations, eUseAlgorithme.kForward);
            Assert.That(NearlyEqual(result, 0.0277625F), "Problem ! result = " + result);

            result = m_HiddenMarkovModel.MEvaluate(observations, eUseAlgorithme.kBackward);
            Assert.That(NearlyEqual(result, 0.0277625F), "Problem ! result = " + result);

            result = m_HiddenMarkovModel.MEvaluate(observations, eUseAlgorithme.kForwardAndBackward, 0);
            Assert.That(NearlyEqual(result, 0.0277625F), "Problem ! result = " + result);
            result = m_HiddenMarkovModel.MEvaluate(observations, eUseAlgorithme.kForwardAndBackward, 1);
            Assert.That(NearlyEqual(result, 0.0277625F), "Problem ! result = " + result);
            result = m_HiddenMarkovModel.MEvaluate(observations, eUseAlgorithme.kForwardAndBackward, 2);
            Assert.That(NearlyEqual(result, 0.0277625F), "Problem ! result = " + result);
            result = m_HiddenMarkovModel.MEvaluate(observations, eUseAlgorithme.kForwardAndBackward, 3);
            Assert.That(NearlyEqual(result, 0.0277625F), "Problem ! result = " + result);
            result = m_HiddenMarkovModel.MEvaluate(observations, eUseAlgorithme.kForwardAndBackward, 4);
            Assert.That(NearlyEqual(result, 0.0277625F), "Problem ! result = " + result);
        }

        [Test]
        public void AKN_HMM_Learning_UT() 
		{
            AKN_HiddenMarkovModel<String> m_HiddenMarkovModel = new AKN_HiddenMarkovModel<String>(3, 2);
            m_HiddenMarkovModel.MSetStates(new float[] { 0.656F, 0.344F, 0.0F }, new string[] { "Etat_1", "Etat_2", "Etat_3" });
            m_HiddenMarkovModel.MSetTransitions(new float[,] { { 0.346F, 0.365F, 0.289F }, { 0.159F, 0.514F, 0.327F }, { 0.377F, 0.259F, 0.364F } });
            m_HiddenMarkovModel.MSetObservations(new String[] { "Pile", "Face" });
            m_HiddenMarkovModel.MSetEmissions(new float[,] { { 1.0F, 0.0F }, { 0.631F, 0.369F }, { 0.0F, 1.0F } });

            String[] observation = new String[] { "Pile", "Face", "Face", "Pile", "Pile" };

            m_HiddenMarkovModel.MUpdate(observation, 0, 15);

            float p = m_HiddenMarkovModel.MEvaluate(observation, eUseAlgorithme.kForward);

            Assert.That(NearlyEqual(p, (float)(0.2479884)), "Problem  = " + (p - 0.2479884f));

            Assert.That(NearlyEqual(m_HiddenMarkovModel["Etat_1"].m_startProbability, 1.0F), "Problem !" + m_HiddenMarkovModel["Etat_1"].m_startProbability);
            Assert.That(NearlyEqual(m_HiddenMarkovModel["Etat_2"].m_startProbability, 0.0F), "Problem !" + m_HiddenMarkovModel["Etat_2"].m_startProbability);
            Assert.That(NearlyEqual(m_HiddenMarkovModel["Etat_3"].m_startProbability, 0.0F), "Problem !" + m_HiddenMarkovModel["Etat_3"].m_startProbability);

            Assert.That(NearlyEqual(m_HiddenMarkovModel["Etat_1", "Etat_1"], 0.0F), "Problem !" + m_HiddenMarkovModel["Etat_1", "Etat_1"]);
            Assert.That(NearlyEqual(m_HiddenMarkovModel["Etat_1", "Etat_2"], 0.0F), "Problem !" + m_HiddenMarkovModel["Etat_1", "Etat_2"]);
            Assert.That(NearlyEqual(m_HiddenMarkovModel["Etat_1", "Etat_3"], 1.0F), "Problem !" + m_HiddenMarkovModel["Etat_1", "Etat_3"]);
            Assert.That(NearlyEqual(m_HiddenMarkovModel["Etat_2", "Etat_1"], 0.2119321F), "Problem !" + m_HiddenMarkovModel["Etat_2", "Etat_1"]);
            Assert.That(NearlyEqual(m_HiddenMarkovModel["Etat_2", "Etat_2"], 0.788068F), "Problem !" + m_HiddenMarkovModel["Etat_2", "Etat_2"]);
            Assert.That(NearlyEqual(m_HiddenMarkovModel["Etat_2", "Etat_3"], 0.0F), "Problem !" + m_HiddenMarkovModel["Etat_2", "Etat_3"]);
            Assert.That(NearlyEqual(m_HiddenMarkovModel["Etat_3", "Etat_1"], 0.0F), "Problem !" + m_HiddenMarkovModel["Etat_3", "Etat_1"]);
            Assert.That(NearlyEqual(m_HiddenMarkovModel["Etat_3", "Etat_2"], 0.5147933F), "Problem !" + m_HiddenMarkovModel["Etat_3", "Etat_2"]);
            Assert.That(NearlyEqual(m_HiddenMarkovModel["Etat_3", "Etat_3"], 0.4852067F), "Problem !" + m_HiddenMarkovModel["Etat_3", "Etat_3"]);

            Assert.That(NearlyEqual(m_HiddenMarkovModel.MGetEmission("Etat_1", "Pile"), 1.0F), "Problem !" + m_HiddenMarkovModel.MGetEmission("Etat_1", "Pile"));
            Assert.That(NearlyEqual(m_HiddenMarkovModel.MGetEmission("Etat_1", "Face"), 0.0F), "Problem !" + m_HiddenMarkovModel.MGetEmission("Etat_1", "Face"));
            Assert.That(NearlyEqual(m_HiddenMarkovModel.MGetEmission("Etat_2", "Pile"), 0.9686517F), "Problem !" + m_HiddenMarkovModel.MGetEmission("Etat_2", "Pile"));
            Assert.That(NearlyEqual(m_HiddenMarkovModel.MGetEmission("Etat_2", "Face"), 0.03134831F), "Problem !" + m_HiddenMarkovModel.MGetEmission("Etat_2", "Face"));
            Assert.That(NearlyEqual(m_HiddenMarkovModel.MGetEmission("Etat_3", "Pile"), 0.0F), "Problem !" + m_HiddenMarkovModel.MGetEmission("Etat_3", "Pile"));
            Assert.That(NearlyEqual(m_HiddenMarkovModel.MGetEmission("Etat_3", "Face"), 1.0F), "Problem !" + m_HiddenMarkovModel.MGetEmission("Etat_3", "Face"));
        }

        [Test]
        public void AKN_HMM_Decoding_UT() 
        {
            AKN_HiddenMarkovModel<String> m_HiddenMarkovModel = new AKN_HiddenMarkovModel<String>(3, 2);
            m_HiddenMarkovModel.MSetStates(new float[] { 0.656F, 0.344F, 0.0F }, new string[] { "Etat_1", "Etat_2", "Etat_3" });
            m_HiddenMarkovModel.MSetTransitions(new float[,] { { 0.346F, 0.365F, 0.289F }, { 0.159F, 0.514F, 0.327F }, { 0.377F, 0.259F, 0.364F } });
            m_HiddenMarkovModel.MSetObservations(new String[] { "Pile", "Face" });
            m_HiddenMarkovModel.MSetEmissions(new float[,] { { 1.0F, 0.0F }, { 0.631F, 0.369F }, { 0.0F, 1.0F } });

            String[] observation = new String[] { "Pile", "Face", "Face", "Pile", "Pile" };

            int[] Path = m_HiddenMarkovModel.MDecode(observation);

            Assert.That(Path.Length == 5, "Problem ! " + Path.Length);
            Assert.That(Path[0] == 0, "Problem ! " + Path[0]);
            Assert.That(Path[1] == 2, "Problem ! " + Path[1]);
            Assert.That(Path[2] == 2, "Problem ! " + Path[2]);
            Assert.That(Path[3] == 0, "Problem ! " + Path[3]);
            Assert.That(Path[4] == 0, "Problem ! " + Path[4]);
        }

        public static bool NearlyEqual(float f1, float f2)
        {
            return System.Math.Abs(f1 - f2) < 0.0000001f;
        }

    }
}