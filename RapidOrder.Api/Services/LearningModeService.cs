namespace RapidOrder.Api.Services
{
    public class LearningModeService
    {
        public bool IsLearningModeEnabled { get; private set; }

        public void EnableLearningMode()
        {
            IsLearningModeEnabled = true;
        }

        public void DisableLearningMode()
        {
            IsLearningModeEnabled = false;
        }
    }
}
