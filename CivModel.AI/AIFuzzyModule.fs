namespace CivModel.AI

module AIFuzzyModule =
    let fuzzySystem = FuzzySystem()

    // Sets

    let DeltaUnitSets =
        fuzzySystem.CreateSetsByList [
            "DeltaUnitHigh", [ 0.000f; 50.000f; infinityf ];
            "DeltaUnitMedium", [ -70.000f; -50.000f; 50.000f; 70.000f ];
            "DeltaUnitLow", [ -infinityf; -50.000f; 0.000f ];
        ]
    let RemainGoldSets =
        fuzzySystem.CreateSetsByList [
            "RemainGoldVeryHigh", [ 500.000f; 1000.000f; infinityf ];
            "RemainGoldHigh", [ 0.000f; 500.000f; 1000.000f; 1500.000f ];
            "RemainGoldMedium", [ -750.000f; -250.000f; 250.000f; 750.000f ];
            "RemainGoldLow", [ -1500.000f; -1000.000f; -500.000f; 0.000f ];
            "RemainGoldVeryLow", [ -infinityf; -1000.000f; -500.000f ];
        ]
    let RemainLaborSets =
        fuzzySystem.CreateSetsByList [
            "RemainLaborVeryHigh", [ 500.000f; 1000.000f; infinityf ];
            "RemainLaborHigh", [ 250.000f; 500.000f; 750.000f; 1000.000f ];
            "RemainLaborMedium", [ 50.000f; 100.000f; 250.000f; 500.000f ];
            "RemainLaborLow", [ 10.0f; 50.0f; 50.000f; 100.000f ];
            "RemainLaborVeryLow", [ 0.000f; 0.000f; 25.000f; 50.000f ];
        ]
    let CitySets =
        fuzzySystem.CreateSetsByList [
            "CityHigh", [ 10.000f; 20.000f; infinityf ];
            "CityMedium", [ -20.000f; -10.000f; 10.000f; 20.000f ];
            "CityLow", [ -infinityf; -20.000f; -10.000f ];
        ]
    let FightingUnitNumSets =
        fuzzySystem.CreateSetsByList [
            "FightingUnitNumHigh", [ 25.000f; 50.000f; infinityf ];
            "FightingUnitNumMedium", [ 0.000f; 25.000f; 50.000f; 75.000f ];
            "FightingUnitNumLow", [ -infinityf; 0.000f; 25.000f ];
        ]
    let EnemDistSets =
        fuzzySystem.CreateSetsByList [
            "EnemDistLow", [ -infinityf; 3.000f; 5.000f ];
            "EnemDistHigh", [ 4.000f; 6.000f; infinityf ];
        ]
    let DeltaHappyGoalSets =
        fuzzySystem.CreateSetsByList [
            "DeltaHappyGoalHigh", [ 0.000f; 0.000f; 200.f ];
            "DeltaHappyGoalLow", [ -200.f; 0.000f; 0.000f ];
        ]
    let HSets =
        fuzzySystem.CreateSetsByList [
            "HVeryHigh", [ 50.000f; 75.000f; 100.000f; 100.000f ];
            "HHigh", [ 25.000f; 50.000f; 75.000f; 100.000f ];
            "HMedium", [ -50.000f; -25.000f; 25.000f; 50.000f ];
            "HLow", [ -100.000f; -75.000f; -50.000f; -25.000f ];
            "HVeryLow", [ -100.000f; -100.000f; -75.000f; -50.000f ];
        ]
    let TechLostSets =
        fuzzySystem.CreateSetsByList [
            "TechLostHigh", [ 2500.000f; 5000.000f; infinityf ];
            "TechLostMedium", [ 0.000f; 2500.000f; 5000.000f; 7500.000f ];
            "TechLostLow", [ 0.000f; 0.000f; 2500.000f; 5000.000f ];
        ]
    let GoldSets =
        fuzzySystem.CreateSetsByList [
            "GoldHigh", [ 2500.000f; 5000.000f; infinityf ];
            "GoldNormal", [ 0.000f; 2500.000f; 5000.000f; 7500.000f ];
            "GoldLow", [ 0.000f; 0.000f; 2500.000f; 5000.000f ];
        ]
    let TechSets =
        fuzzySystem.CreateSetsByList [
            "TechHigh", [ 10000.000f; 20000.000f; infinityf ];
            "TechNormal", [ 5000.000f; 10000.000f; 20000.000f; 25000.000f ];
            "TechLow", [ 0.000f; 0.000f; 5000.000f; 10000.000f ];
        ]
    let LaborSets =
        fuzzySystem.CreateSetsByList [
            "LaborHigh", [ 1000.000f; 2000.000f; infinityf ];
            "LaborNormal", [ 250.000f; 500.000f; 1000.000f; 1500.000f ];
            "LaborLow", [ 0.000f; 100.000f; 250.000f; 500.000f ];
        ]
    let TSets =
        fuzzySystem.CreateSetsByList [
            "THigh", [ 1.000f; 1.500f; 2.000f; 2.000f ];
            "TMedium", [ 0.500f; 0.750f; 1.250f; 1.500f ];
            "TLow", [ 0.000f; 0.000f; 0.500f; 1.000f ];
        ]
    let UnitNumSets =
        fuzzySystem.CreateSetsByList [
            "UnitNumHigh", [ 0.500f; 0.750f; 1.000f; 1.000f ];
            "UnitNumMedium", [ 0.200f; 0.400f; 0.600f; 0.800f ];
            "UnitNumLow", [ 0.000f; 0.100f; 0.200f; 0.250f ];
            "UnitNumVeryLow", [ 0.000f ];
        ]
    let LSets =
        fuzzySystem.CreateSetsByList [
            "LHigh", [ 1.000f ];
            "LMedium", [ 0.500f; 0.700f; 0.900f ];
            "LLow", [ 0.400f; 0.500f; 0.600f ];
            "LVeryLow", [ 0.000f ];
        ]

    let ResultSets =
         [ -infinityf; -0.8f; -0.6f; -0.2f; 0.2f; 0.8f; infinityf ]
         |> fuzzySystem.CreateSetsByLevel "" [ "VeryLow"; "Low"; "Medium"; "High"; "VeryHigh" ] 3 1

    // Variables

    let DeltaFightingUnitNum = DeltaUnitSets |> fuzzySystem.CreateVariable "DeltaFightingUnitNum" -infinityf infinityf
    let RemainingGold = RemainGoldSets |> fuzzySystem.CreateVariable "RemainingGold" -infinityf infinityf
    let RemainingLabor = RemainLaborSets |> fuzzySystem.CreateVariable "RemainingLabor" 0.f infinityf
    let NeedFightingUnit = ResultSets |> fuzzySystem.CreateVariable "NeedFightingUnit" -infinityf infinityf
    let DeltaCityNum = CitySets |> fuzzySystem.CreateVariable "DeltaCityNum" -infinityf infinityf
    let NeedCity = ResultSets |> fuzzySystem.CreateVariable "NeedCity" -infinityf infinityf
    let NeedPioneer = ResultSets |> fuzzySystem.CreateVariable "NeedPioneer" -infinityf infinityf
    let EnemyFightingUnitNum = EnemDistSets |> fuzzySystem.CreateVariable "EnemyFightingUnitNum" 0.f infinityf
    let AllMyUnitEnemDist = EnemDistSets |> fuzzySystem.CreateVariable "AllMyUnitEnemDist" 0.f infinityf
    let DeltaHappyGoal = DeltaHappyGoalSets |> fuzzySystem.CreateVariable "DeltaHappyGoal" -200.f 200.f
    let SetEconInvesttoFull = ResultSets |> fuzzySystem.CreateVariable "SetEconInvesttoFull" -infinityf infinityf
    let SetEconInvesttoDouble = ResultSets |> fuzzySystem.CreateVariable "SetEconInvesttoDouble" -infinityf infinityf
    let Gold = GoldSets |> fuzzySystem.CreateVariable "Gold" 0.f infinityf
    let Tech = TechSets |> fuzzySystem.CreateVariable "Tech" 0.f infinityf
    let NeedGold = ResultSets |> fuzzySystem.CreateVariable "NeedGold" -infinityf infinityf
    let NeedLabor = ResultSets |> fuzzySystem.CreateVariable "NeedLabor" -infinityf infinityf
    let NeedTech = ResultSets |> fuzzySystem.CreateVariable "NeedTech" -infinityf infinityf
    let TechLost = TechLostSets |> fuzzySystem.CreateVariable "TechLost" 0.f infinityf
    let TechInvest = TSets |> fuzzySystem.CreateVariable "TechInvest" 0.f infinityf
    let DmgUnitNum = UnitNumSets |> fuzzySystem.CreateVariable "DmgUnitNum" 0.f 1.f
    let NeedLogistics = ResultSets |> fuzzySystem.CreateVariable "NeedLogistics" -infinityf infinityf
    let Logistics = LSets |> fuzzySystem.CreateVariable "Logistics" -infinityf infinityf
    let NeedMilitaryBuilding = ResultSets |> fuzzySystem.CreateVariable "NeedMilitaryBuilding" -infinityf infinityf

    let RuleString = """
IF RemainingGold is RemainGoldVeryLow OR RemainingGold is RemainGoldLow THEN NeedFightingUnit is Low
IF RemainingGold is RemainGoldMedium THEN NeedFightingUnit is Medium
IF RemainingGold is RemainGoldHigh OR RemainingGold is RemainGoldVeryHigh THEN NeedFightingUnit is High
IF RemainingLabor is RemainLaborVeryLow OR RemainingLabor is RemainLaborLow THEN NeedFightingUnit is Low
IF RemainingLabor is RemainLaborMedium THEN NeedFightingUnit is Medium
IF RemainingLabor is RemainLaborHigh OR RemainingLabor is RemainLaborVeryHigh THEN NeedFightingUnit is High
IF NeedCity is Low THEN NeedPioneer is VeryLow
IF NeedCity is High THEN NeedPioneer is VeryHigh
IF RemainingGold is RemainGoldVeryLow OR RemainingGold is RemainGoldLow THEN NeedPioneer is Low
IF RemainingGold is RemainGoldMedium THEN NeedPioneer is Medium
IF RemainingGold is RemainGoldHigh OR RemainingGold is RemainGoldVeryHigh THEN NeedPioneer is High
IF RemainingLabor is RemainLaborVeryLow OR RemainingLabor is RemainLaborLow THEN NeedPioneer is Low
IF RemainingLabor is RemainLaborMedium THEN NeedPioneer is Medium
IF RemainingLabor is RemainLaborHigh OR RemainingLabor is RemainLaborVeryHigh THEN NeedPioneer is High
IF AllMyUnitEnemDist is EnemDistLow THEN NeedMilitaryBuilding is High
IF AllMyUnitEnemDist is EnemDistHigh THEN NeedMilitaryBuilding is Low
IF RemainingGold is RemainGoldVeryLow OR RemainingGold is RemainGoldLow THEN NeedMilitaryBuilding is Low
IF RemainingGold is RemainGoldMedium THEN NeedMilitaryBuilding is Medium
IF RemainingGold is RemainGoldHigh OR RemainingGold is RemainGoldVeryHigh THEN NeedMilitaryBuilding is High
IF RemainingLabor is RemainLaborVeryLow OR RemainingLabor is RemainLaborLow THEN NeedMilitaryBuilding is Low
IF RemainingLabor is RemainLaborMedium THEN NeedMilitaryBuilding is Medium
IF RemainingLabor is RemainLaborHigh OR RemainingLabor is RemainLaborVeryHigh THEN NeedMilitaryBuilding is High
IF DeltaHappyGoal is DeltaHappyGoalHigh THEN SetEconInvesttoFull is VeryHigh
IF DeltaHappyGoal is DeltaHappyGoalHigh THEN SetEconInvesttoDouble is VeryLow
IF DeltaHappyGoal is DeltaHappyGoalLow THEN SetEconInvesttoDouble is VeryHigh
IF DeltaHappyGoal is DeltaHappyGoalLow THEN SetEconInvesttoFull is VeryLow
IF RemainingGold is RemainGoldVeryLow OR RemainingGold is RemainGoldLow THEN NeedGold is VeryHigh
IF RemainingGold is RemainGoldVeryHigh OR RemainingGold is RemainGoldHigh THEN NeedGold is VeryLow
IF RemainingGold is RemainGoldMedium THEN NeedGold is Medium
IF RemainingLabor is RemainLaborVeryLow OR RemainingLabor is RemainLaborLow THEN NeedLabor is VeryHigh
IF RemainingLabor is RemainLaborVeryHigh OR RemainingLabor is RemainLaborHigh THEN NeedLabor is VeryLow
IF RemainingLabor is RemainLaborMedium THEN NeedLabor is Medium
IF TechLost is TechLostHigh THEN NeedTech is VeryHigh
IF TechLost is TechLostMedium THEN NeedTech is High
IF TechLost is TechLostLow THEN NeedTech is Medium
IF Tech is TechHigh THEN NeedTech is Low
IF Tech is TechNormal THEN NeedTech is Medium
IF Tech is TechLow THEN NeedTech is High
IF NeedGold is VeryHigh THEN NeedTech is VeryLow
IF NeedTech is High THEN TechInvest is THigh
IF NeedTech is Medium THEN TechInvest is TMedium
IF NeedTech is Low THEN TechInvest is TLow
IF DmgUnitNum is UnitNumHigh THEN NeedLogistics is High
IF DmgUnitNum is UnitNumMedium THEN NeedLogistics is Medium
IF DmgUnitNum is UnitNumLow THEN NeedLogistics is Low
IF DmgUnitNum is UnitNumVeryLow THEN NeedLogistics is VeryLow
IF RemainingLabor is RemainLaborHigh THEN NeedLogistics is Medium
IF NeedLogistics is High THEN Logistics is LHigh
IF NeedLogistics is Medium THEN Logistics is LMedium
IF NeedLogistics is Low THEN Logistics is LLow
IF NeedLogistics is VeryLow THEN Logistics is LVeryLow
"""
