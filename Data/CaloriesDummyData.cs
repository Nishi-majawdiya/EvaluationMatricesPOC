using System.Collections.Generic;
using EvaluationMatricesPOC.Models;

namespace EvaluationMatricesPOC.Data
{
    public static class CaloriesDummyData
    {
        public static List<CaloriesInput> GetData()
        {
            return new List<CaloriesInput>
            {
                new CaloriesInput { Weight = 60, Duration = 30, HeartRate = 110, ExerciseType = "Running", CaloriesBurned = 200 },
                new CaloriesInput { Weight = 70, Duration = 45, HeartRate = 120, ExerciseType = "Cycling", CaloriesBurned = 350 },
                new CaloriesInput{ Weight = 80, Duration = 60, HeartRate = 130, ExerciseType = "Swimming", CaloriesBurned = 500 },
                new CaloriesInput{ Weight = 55, Duration = 20, HeartRate = 100, ExerciseType = "Walking", CaloriesBurned = 120 },
                new CaloriesInput { Weight = 65, Duration = 40, HeartRate = 115, ExerciseType = "Yoga", CaloriesBurned = 180 },
                new CaloriesInput { Weight = 60, Duration = 30, HeartRate = 110, ExerciseType = "Running", CaloriesBurned = 200 },
                new CaloriesInput{ Weight = 50, Duration = 45, HeartRate = 130, ExerciseType = "Cycling", CaloriesBurned = 550 },
                new CaloriesInput { Weight = 95, Duration = 60, HeartRate = 125, ExerciseType = "Swimming", CaloriesBurned = 500 },
                new CaloriesInput { Weight = 55, Duration = 20, HeartRate = 110, ExerciseType = "Yoga", CaloriesBurned = 120 },
                new CaloriesInput { Weight = 72, Duration = 50, HeartRate = 115, ExerciseType = "Running", CaloriesBurned = 180 },


            };
        }
    }
}
