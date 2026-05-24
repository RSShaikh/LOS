using LOS.Models;

namespace LOS.Interfaces
{
    public interface IScoreCardService
    {
        List<ScoreCard> FetchScoreCard();

        string EditScoreCard(int id, string riskCategory);

        void GenerateOrUpdateScoreCards();

        ScoreCard GetScoreCardById(int id);


    }
}
