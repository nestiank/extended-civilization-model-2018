namespace CivModel.AI

module AIFuzzyModule =
    let fuzzy = FuzzySystem()

    // Sets

    let DeltaUnitSets =
        fuzzy.CreateSetsByList [
            "DeltaUnitHigh", [ 0.000f; 50.000f; infinityf ];
            "DeltaUnitMedium", [ -70.000f; -50.000f; 50.000f; 70.000f ];
            "DeltaUnitLow", [ -infinityf; -50.000f; 0.000f ];
        ]
    let RemainGoldSets =
        fuzzy.CreateSetsByList [
            "RemainGoldVeryHigh", [ 500.000f; 1000.000f; infinityf ];
            "RemainGoldHigh", [ 0.000f; 500.000f; 1000.000f; 1500.000f ];
            "RemainGoldMedium", [ -750.000f; -250.000f; 250.000f; 750.000f ];
            "RemainGoldLow", [ -1500.000f; -1000.000f; -500.000f; 0.000f ];
            "RemainGoldVeryLow", [ -infinityf; -1000.000f; -500.000f ];
        ]
    let RemainLaborSets =
        fuzzy.CreateSetsByList [
            "RemainLaborVeryHigh", [ 500.000f; 1000.000f; infinityf ];
            "RemainLaborHigh", [ 250.000f; 500.000f; 750.000f; 1000.000f ];
            "RemainLaborMedium", [ 50.000f; 100.000f; 250.000f; 500.000f ];
            "RemainLaborLow", [ 0.000f; 0.000f; 50.000f; 100.000f ];
            "RemainLaborVeryLow", [ -infinityf; -1000.000f; -500.000f ];
        ]
    let CitySets =
        fuzzy.CreateSetsByList [
            "CityHigh", [ 10.000f; 20.000f; infinityf ];
            "CityMedium", [ -20.000f; -10.000f; 10.000f; 20.000f ];
            "CityLow", [ -infinityf; -20.000f; -10.000f ];
        ]
    let FightingUnitNumSets =
        fuzzy.CreateSetsByList [
            "FightingUnitNumHigh", [ 25.000f; 50.000f; infinityf ];
            "FightingUnitNumMedium", [ 0.000f; 25.000f; 50.000f; 75.000f ];
            "FightingUnitNumLow", [ -infinityf; 0.000f; 25.000f ];
        ]
    let EnemDistSets =
        fuzzy.CreateSetsByList [
            "EnemDistLow", [ -infinityf; 3.000f; 5.000f ];
            "EnemDistHigh", [ 4.000f; 6.000f; infinityf ];
        ]
    let DeltaHappyGoalSets =
        fuzzy.CreateSetsByList [
            "DeltaHappyGoalHigh", [ 0.000f; 0.000f; infinityf ];
            "DeltaHappyGoalLow", [ -infinityf; 0.000f; 0.000f ];
        ]
    let HSets =
        fuzzy.CreateSetsByList [
            "HVeryHigh", [ 50.000f; 75.000f; 100.000f; 100.000f ];
            "HHigh", [ 25.000f; 50.000f; 75.000f; 100.000f ];
            "HMedium", [ -50.000f; -25.000f; 25.000f; 50.000f ];
            "HLow", [ -100.000f; -75.000f; -50.000f; -25.000f ];
            "HVeryLow", [ -100.000f; -100.000f; -75.000f; -50.000f ];
        ]
    let TechLostSets =
        fuzzy.CreateSetsByList [
            "TechLostHigh", [ 2500.000f; 5000.000f; infinityf ];
            "TechLostMedium", [ 0.000f; 2500.000f; 5000.000f; 7500.000f ];
            "TechLostLow", [ 0.000f; 0.000f; 2500.000f; 5000.000f ];
        ]
    let GoldSets =
        fuzzy.CreateSetsByList [
            "GoldHigh", [ 2500.000f; 5000.000f; infinityf ];
            "GoldNormal", [ 0.000f; 2500.000f; 5000.000f; 7500.000f ];
            "GoldLow", [ 0.000f; 0.000f; 2500.000f; 5000.000f ];
        ]
    let TechSets =
        fuzzy.CreateSetsByList [
            "TechHigh", [ 10000.000f; 20000.000f; infinityf ];
            "TechNormal", [ 5000.000f; 10000.000f; 20000.000f; 25000.000f ];
            "TechLow", [ 0.000f; 0.000f; 5000.000f; 10000.000f ];
        ]
    let LaborSets =
        fuzzy.CreateSetsByList [
            "LaborHigh", [ 1000.000f; 2000.000f; infinityf ];
            "LaborNormal", [ 250.000f; 500.000f; 1000.000f; 1500.000f ];
            "LaborLow", [ 0.000f; 100.000f; 250.000f; 500.000f ];
        ]
    let TSets =
        fuzzy.CreateSetsByList [
            "THigh", [ 1.000f; 1.500f; 2.000f; 2.000f ];
            "TMedium", [ 0.500f; 0.750f; 1.250f; 1.500f ];
            "TLow", [ 0.000f; 0.000f; 0.500f; 1.000f ];
        ]
    let UnitNumSets =
        fuzzy.CreateSetsByList [
            "UnitNumHigh", [ 0.500f; 0.750f; 1.000f; 1.000f ];
            "UnitNumMedium", [ 0.200f; 0.400f; 0.600f; 0.800f ];
            "UnitNumLow", [ 0.000f; 0.100f; 0.200f; 0.250f ];
            "UnitNumVeryLow", [ 0.000f ];
        ]
    let LSets =
        fuzzy.CreateSetsByList [
            "LHigh", [ 1.000f ];
            "LMedium", [ 0.500f; 0.700f; 0.900f ];
            "LLow", [ 0.400f; 0.500f; 0.600f ];
            "LVeryLow", [ 0.000f ];
        ]

    let ResultSets =
         [ -infinityf; -0.8f; -0.6f; -0.2f; 0.2f; 0.8f; infinityf ]
         |> fuzzy.CreateSetsByLevel "" [ "VeryLow"; "Low"; "Normal"; "High"; "VeryHigh" ] 3 1

    // Variables

    let DeltaFightingUnitNum = DeltaUnitSets |> fuzzy.CreateVariable "DeltaFightingUnitNum" -infinityf infinityf
    let RemainingGold = RemainGoldSets |> fuzzy.CreateVariable "RemainingGold" -infinityf infinityf
    let RemainingLabor = RemainLaborSets |> fuzzy.CreateVariable "RemainingLabor" 0.f infinityf
    let NeedFightingUnit = ResultSets |> fuzzy.CreateVariable "NeedFightingUnit" -infinityf infinityf
    let BuildFightingUnit = ResultSets |> fuzzy.CreateVariable "BuildFightingUnit" -infinityf infinityf
    let DeltaCityNum = ResultSets |> fuzzy.CreateVariable "DeltaCityNum" -infinityf infinityf
    let NeedCity = ResultSets |> fuzzy.CreateVariable "NeedCity" -infinityf infinityf
    let NeedPioneer = ResultSets |> fuzzy.CreateVariable "NeedPioneer" -infinityf infinityf
    let BuildCityCenter = ResultSets |> fuzzy.CreateVariable "BuildCityCenter" -infinityf infinityf
    let BuildPioneer = ResultSets |> fuzzy.CreateVariable "BuildPioneer" -infinityf infinityf
    let EnemyFightingUnitNum = EnemDistSets |> fuzzy.CreateVariable "EnemyFightingUnitNum" 0.f infinityf
    let AllMyUnitEnumDist = EnemDistSets |> fuzzy.CreateVariable "AllMyUnitEnumDist" 0.f infinityf
    let DeltaHappyGoal = DeltaHappyGoalSets |> fuzzy.CreateVariable "DeltaHappyGoal" -200.f 200.f
    let SetEconInvesttoFull = ResultSets |> fuzzy.CreateVariable "SetEconInvesttoFull" -infinityf infinityf
    let SetEconInvesttoDouble = ResultSets |> fuzzy.CreateVariable "SetEconInvesttoDouble" -infinityf infinityf
    let HappinessGoal = HSets |> fuzzy.CreateVariable "HappinessGoal" -100.f 100.f
    let Gold = GoldSets |> fuzzy.CreateVariable "Gold" 0.f infinityf
    let Tech = TechSets |> fuzzy.CreateVariable "Tech" 0.f infinityf
    let NeedGold = ResultSets |> fuzzy.CreateVariable "NeedGold" -infinityf infinityf
    let NeedLabor = ResultSets |> fuzzy.CreateVariable "NeedLabor" -infinityf infinityf
    let NeedTech = ResultSets |> fuzzy.CreateVariable "NeedTech" -infinityf infinityf
    let TechLost = TechLostSets |> fuzzy.CreateVariable "TechLost" 0.f infinityf
    let TechInvest = ResultSets |> fuzzy.CreateVariable "TechInvest" 0.f infinityf
    let DmgUnitNum = ResultSets |> fuzzy.CreateVariable "DmgUnitNum" 0.f 1.f
    let NeedLogistics = ResultSets |> fuzzy.CreateVariable "NeedLogistics" -infinityf infinityf
    let Logistics = LSets |> fuzzy.CreateVariable "Logistics" -infinityf infinityf

    let RuleString = """
IF Research is ResearchLow THEN NeedLaboratory is High
IF Research is ResearchNormal THEN NeedLaboratory is Normal
IF Research is ResearchHigh THEN NeedLaboratory is VeryLow
IF Labor is LaborLow THEN NeedFactory is VeryHigh
IF Labor is LaborNormal THEN NeedFactory is High
IF Labor is LaborHigh THEN NeedFactory is VeryLow
IF NeedProduction is High AND ProductionLaborCost is CostLow AND ProductionGoldCost is CostLow THEN BuildProduction is VeryHigh
IF NeedProduction is High AND ProductionLaborCost is CostNormal AND ProductionGoldCost is CostLow THEN BuildProduction is High
IF NeedProduction is High AND ProductionLaborCost is CostLow AND ProductionGoldCost is CostNormal THEN BuildProduction is High
IF NeedProduction is High AND ProductionLaborCost is CostNormal AND ProductionGoldCost is CostNormal THEN BuildProduction is Normal
IF NeedProduction is High AND ProductionLaborCost is CostNormal AND ProductionGoldCost is CostHigh THEN BuildProduction is Low
IF NeedProduction is High AND ProductionLaborCost is CostHigh AND ProductionGoldCost is CostNormal THEN BuildProduction is Low
IF NeedProduction is High AND ProductionLaborCost is CostHigh AND ProductionGoldCost is CostHigh THEN BuildProduction is VeryLow
"""
