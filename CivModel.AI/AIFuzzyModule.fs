namespace CivModel.AI

open System.Net

module AIFuzzyModule =
    let fuzzySystem = FuzzySystem()

    // Common Sets
     
    let ResultSets =
         [ -infinityf; -0.5f; -0.30f; 0.0f; 0.30f; 0.5f; infinityf ]
         |> fuzzySystem.CreateSetsByLevel "" [ "VeryLow"; "Low"; "Medium"; "High"; "VeryHigh" ] 3 1
    let createOutputVariable name = ResultSets |> fuzzySystem.CreateVariable name -1.0f 1.0f

    let RemainGoldSets =
        fuzzySystem.CreateSetsByList [
            "RemainGoldVeryHigh", [ 500.000f; 1000.000f; infinityf ];
            "RemainGoldHigh", [ 0.000f; 250.000f; 1000.000f; 1500.000f ];
            "RemainGoldMedium", [ -750.000f; -500.000f; 250.000f; 750.000f ];
            "RemainGoldLow", [ -1500.000f; -1000.000f; -500.000f; 0.000f ];
            "RemainGoldVeryLow", [ -infinityf; -1000.000f; -500.000f ];
        ]
    let RemainLaborSets =
        fuzzySystem.CreateSetsByList [
            "RemainLaborVeryHigh", [ 500.000f; 1000.000f; infinityf ];
            "RemainLaborHigh", [ 250.000f; 500.000f; 750.000f; 1000.000f ];
            "RemainLaborMedium", [ 50.000f; 100.000f; 250.000f; 500.000f ];
            "RemainLaborLow", [ 10.0f; 50.0f; 50.000f; 100.000f ];
            "RemainLaborVeryLow", [ -infinityf; 25.000f; 50.000f ];
        ]
    
    let EnemDistSets =
        fuzzySystem.CreateSetsByList [
            "EnemDistLow", [ -infinityf; 3.000f; 5.000f ];
            "EnemDistHigh", [ 4.000f; 6.000f; infinityf ];
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
            "TechLostLow", [ -infinityf; 2500.000f; 5000.000f ];
        ]
    let GoldSets =
        fuzzySystem.CreateSetsByList [
            "GoldHigh", [ 2500.000f; 5000.000f; infinityf ];
            "GoldMedium", [ 0.000f; 2500.000f; 5000.000f; 7500.000f ];
            "GoldLow", [ -infinityf; 2500.000f; 5000.000f ];
        ]
    let TechSets =
        fuzzySystem.CreateSetsByList [
            "TechHigh", [ 10000.000f; 20000.000f; infinityf ];
            "TechMedium", [ 5000.000f; 10000.000f; 20000.000f; 25000.000f ];
            "TechLow", [ -infinityf; 5000.000f; 10000.000f ];
        ]
    let LaborSets =
        fuzzySystem.CreateSetsByList [
            "LaborHigh", [ 1000.000f; 2000.000f; infinityf ];
            "LaborMedium", [ 250.000f; 500.000f; 1000.000f; 1500.000f ];
            "LaborLow", [ 0.000f; 100.000f; 250.000f; 500.000f ];
        ]
    let HappySets =
        fuzzySystem.CreateSetsByList [
            "HappyHigh", [ 50.0f; 75.0f; infinityf ];
            "HappyMedium", [ 0.0f;25.0f; 50.0f; 75.0f ];
            "HappyLow", [ -infinityf; 0.0f; 25.0f ];
        ]
    let TSets =
        fuzzySystem.CreateSetsByList [
            "THigh", [ 1.000f; 1.500f; 2.000f; 2.000f ];
            "TMedium", [ 0.500f; 0.750f; 1.250f; 1.500f ];
            "TLow", [ -infinityf; 0.500f; 1.000f ];
        ]
    let UnitNumSets =
        fuzzySystem.CreateSetsByList [
            "UnitNumHigh", [ 0.500f; 0.750f; 1.000f; 1.000f ];
            "UnitNumMedium", [ 0.200f; 0.400f; 0.600f; 0.800f ];
            "UnitNumLow", [ 0.000f; 0.100f; 0.200f; 0.250f ];
            "UnitNumVeryLow", [ -infinityf; 0.100f; 0.150f ];
        ]
    let LSets =
        fuzzySystem.CreateSetsByList [
            "LHigh", [ 0.700f; 0.900f; infinityf ];
            "LMedium", [ 0.500f; 0.700f; 0.900f ];
            "LLow", [ 0.400f; 0.500f; 0.600f ];
            "LVeryLow", [ -infinityf; 0.400f; 0.500f ];
        ]
    
    let DeltaHappyGoalSets =
        fuzzySystem.CreateSetsByList [
            "DeltaHappyGoalHigh", [ 0.000f; 0.000f; 200.f ];
            "DeltaHappyGoalLow", [ -200.f; 0.000f; 0.000f ];
        ]

    let FightingUnitNumSets =
        fuzzySystem.CreateSetsByList [
            "FightingUnitNumHigh", [ 25.000f; 50.000f; infinityf ];
            "FightingUnitNumMedium", [ 0.000f; 25.000f; 50.000f; 75.000f ];
            "FightingUnitNumLow", [ -infinityf; 0.000f; 25.000f ];
        ]

    // Suomi/Hwan Global Sets

    let CitySets =
        fuzzySystem.CreateSetsByList [
            "CityHigh", [ 2.000f; 4.000f; infinityf ];
            "CityMedium", [ -4.000f; -2.000f; 2.000f; 4.000f ];
            "CityLow", [ -infinityf; -4.000f; -2.000f ];
        ]

    let DeltaUnitSets = 
        fuzzySystem.CreateSetsByList [
            "DeltaUnitHigh", [ 0.000f; 5.000f; infinityf ];
            "DeltaUnitMedium", [ -7.000f; -5.000f; 5.000f; 7.000f ];
            "DeltaUnitLow", [ -infinityf; -5.000f; 0.000f ];
        ]

    // Other Zapgguk Global Sets

    let MyCityNumSets = 
        fuzzySystem.CreateSetsByList [
            "MyCityNumHigh", [ 4.000f; 6.000f; infinityf];
            "MyCityNumMedium", [ 2.000f;4.000f;6.000f;8.000f];
            "MyCityNumLow", [-infinityf; 2.000f; 4.000f];
        ]

    // Common MoveTask Sets

    let UnitTypeSets =
        fuzzySystem.CreateSetsByList [
            "FightingUnit", [0.000f];
            "Pioneer", [1.000f];
        ]

    let HPSets =
        fuzzySystem.CreateSetsByList [
            "HPHigh", [0.500f ; 0.750f; infinityf];
            "HPLow", [-infinityf; 0.300f;0.600f];
        ]
    
    let DamageSets =
        fuzzySystem.CreateSetsByList [
            "DamageHigh", [0.300f;0.600f;+infinityf];
            "DamageMedium", [-0.400f;-0.200f;0.200f;0.400f];
            "DamageLow", [-infinityf;-0.600f;-0.300f];
        ]

    let DealSets = 
        fuzzySystem.CreateSetsByList [
            "DealHigh", [0.000f;1.000f;infinityf];
            "DealMedium", [-1.000f;0.000f;1.000f];
            "DealLow", [-infinityf;-1.000f;0.000f];
        ]

    let DistEnemSets =
        fuzzySystem.CreateSetsByList [
            "DistEnemHigh", [0.000f;0.500f;infinityf];
            "DistEnemMedium",[-0.500f;0.000f;0.500f];
            "DistEnemLow", [-infinityf;-0.500f;0.000f];
        ]

    let DistMyCitySets =
        fuzzySystem.CreateSetsByList [
            "DistMyCityHigh", [0.000f;0.500f;infinityf];
            "DistMyCityMedium",[-0.500f;0.000f;0.500f];
            "DistMyCityLow", [-infinityf;-0.500f;0.000f];
        ]

    // Suomi/Hwan MoveTask Sets

    // Zapgguk MoveTask Sets

    let OwnerTypeSets =
        fuzzySystem.CreateSetsByList [
            "Me", [0.000f];
            "Enemy", [1.000f];
            "Ally", [2.000f];
        ]
    
    // Common BuildResourceBuilding Sets

    let ResourceTypeSets =
        fuzzySystem.CreateSetsByList [
            "Gold", [0.000f]
            "Tech",[1.000f]
            "Labor",[2.000f]
            "Happy",[3.000f]
        ]

    // Common DeployTask Sets

    let RsrcBuildingNumSets =
        fuzzySystem.CreateSetsByList [
            "RsrcBuildingNumHigh", [40.000f;80.000f;+infinityf];
            "RsrcBuildingNumLow", [-infinityf;40.000f;80.000f];
        ]
    
    let SpotRsrcBuildingNearSets =
        fuzzySystem.CreateSetsByList [
            "SpotRsrcBuildingNearHigh", [12.000f;24.000f;36.000f;36.000f];
            "SpotRsrcBuildingNearLow", [0.000f;0.000f;12.000f;24.000f];
        ]

    let MyUnitDistSets =
        fuzzySystem.CreateSetsByList [
            "MyUnitDistHigh", [5.000f;8.000f;+infinityf];
            "MyUnitDistLow", [0.000f;0.000f;3.000f;6.000f];
        ]

    let DeployDistEnemSets =
        fuzzySystem.CreateSetsByList [
            "DeployDistEnemHigh", [5.000f;8.000f;+infinityf];
            "DeployDistEnemLow", [0.000f;0.000f;3.000f;6.000f];
        ]


    // Common UseSkillTask Sets

    let SkillEffectTypeSets =
        fuzzySystem.CreateSetsByList [
            "DamageEnemy", [0.000f];
            "RestoreAP", [1.000f];
            "RestoreHP", [2.000f];
            "Buff", [3.000f];
        ]
    let SkillTargetTypeSets =
        fuzzySystem.CreateSetsByList [
            "Me", [0.000f];
            "Enemy", [1.000f];
            "Ally", [2.000f];
        ]
    
    let SpotDamageSets =
        fuzzySystem.CreateSetsByList [
            "SpotDamageHigh", [0.5f;0.8f;+infinityf]
            "SpotDamageLow", [0.0f;0.0f; 0.4f;0.6f]
        ]
    
    let SpotDealSets =
        fuzzySystem.CreateSetsByList [
            "SpotDealHigh", [3.0f;6.0f;+infinityf]
            "SpotDealLow", [0.0f;0.0f;2.0f;4.0f]
        ]

    // Global Common Input Variables

    let RemainingGold = RemainGoldSets |> fuzzySystem.CreateVariable "RemainingGold" -infinityf infinityf
    let RemainingLabor = RemainLaborSets |> fuzzySystem.CreateVariable "RemainingLabor" 0.f infinityf
    let AllMyUnitEnemDist = EnemDistSets |> fuzzySystem.CreateVariable "AllMyUnitEnemDist" 0.f infinityf
    let DeltaHappyGoal = DeltaHappyGoalSets |> fuzzySystem.CreateVariable "DeltaHappyGoal" -200.f 200.f
    let Gold = GoldSets |> fuzzySystem.CreateVariable "Gold" 0.f infinityf
    let Tech = TechSets |> fuzzySystem.CreateVariable "Tech" 0.f infinityf
    let Labor = LaborSets |> fuzzySystem.CreateVariable "Labor" 0.f infinityf
    let Happy = HappySets |> fuzzySystem.CreateVariable "Happy" 0.f infinityf
    let TechLost = TechLostSets |> fuzzySystem.CreateVariable "TechLost" 0.f infinityf
    let TechInvest = TSets |> fuzzySystem.CreateVariable "TechInvest" 0.f infinityf
    let DmgUnitNum = UnitNumSets |> fuzzySystem.CreateVariable "DmgUnitNum" 0.f 1.f
    let Logistics = LSets |> fuzzySystem.CreateVariable "Logistics" -infinityf infinityf

    // Global Common Output Variables

    let NeedFightingUnit = createOutputVariable "NeedFightingUnit"
    let NeedCity = createOutputVariable "NeedCity"
    let NeedPioneer = createOutputVariable "NeedPioneer"
    let NeedMilitaryBuilding = createOutputVariable "NeedMilitaryBuilding"
    let SetEconInvesttoFull = createOutputVariable "SetEconInvesttoFull"
    let SetEconInvesttoDouble = createOutputVariable "SetEconInvesttoDouble"
    
    // Global Common Mid-layer Variables

    let NeedGold = createOutputVariable "NeedGold"
    let NeedLabor = createOutputVariable "NeedLabor"
    let NeedTech = createOutputVariable "NeedTech"
    let NeedLogistics = createOutputVariable "NeedLogistics"
    let NeedHappy = createOutputVariable "NeedHappy"

    // Global Suomi/Hwan Input Variables

    let DeltaFightingUnitNum = DeltaUnitSets |> fuzzySystem.CreateVariable "DeltaFightingUnitNum" -infinityf infinityf
    let DeltaCityNum = CitySets |> fuzzySystem.CreateVariable "DeltaCityNum" -infinityf infinityf
    let EnemyFightingUnitNum = FightingUnitNumSets |> fuzzySystem.CreateVariable "EnemyFightingUnitNum" 0.f infinityf

    // Global Suomi/Hwan Output Variables

    // Global Zapgguk Input Variables

    let MyFightingUnitNum = FightingUnitNumSets |> fuzzySystem.CreateVariable "MyFightingUnitNum" 0.f infinityf
    let MyCityNum = MyCityNumSets |> fuzzySystem.CreateVariable "MyCityNum" 0.f infinityf

    // Global Zapgguk Output Variables

    // MoveTask Common Input Variables

    let MyUnit = UnitTypeSets |> fuzzySystem.CreateVariable "MyUnit" 0.f 1.000f
    let MyUnitHP = HPSets |> fuzzySystem.CreateVariable "MyUnitHP" 0.f 1.000f
    let EnemyUnitHP = HPSets |> fuzzySystem.CreateVariable "EnemyUnitHP" 0.f 1.000f
    let EnemyCityHP = HPSets |> fuzzySystem.CreateVariable "EnemyCityHP" 0.f 1.000f
    let SpotDamage = DamageSets |> fuzzySystem.CreateVariable "SpotDamage" -infinityf infinityf
    let SpotDeal = DealSets |> fuzzySystem.CreateVariable "SpotDeal" -infinityf +infinityf
    let SpotEnemDist = DistEnemSets |> fuzzySystem.CreateVariable "SpotEnemDist" -infinityf +infinityf
    let SpotMyCityDist = DistMyCitySets |> fuzzySystem.CreateVariable "SpotMyCityDist" -infinityf +infinityf
    
    // MoveTask Common Output Variables

    let MoveUnittoSpot = ResultSets |> fuzzySystem.CreateVariable "MoveUnittoSpot" -1.000f 1.000f

    // MoveTask Zap Input Variables

    let SpotOwner = OwnerTypeSets |> fuzzySystem.CreateVariable "SpotOwner" 0.0f 2.0f

    // BuildingTask Common Input Variables

    let BuildingProdResource = ResourceTypeSets |> fuzzySystem.CreateVariable "BuildingProdResource" 0.f 3.000f

    // BuildingTask Common OutputVariables

    let BuildResourceBuilding = ResultSets |> fuzzySystem.CreateVariable "BuildResourceBuilding" -1.000f 1.000f

    // DeployTask Common Input Variables

    let CityatSpotRsrcBuildingNum = RsrcBuildingNumSets |> fuzzySystem.CreateVariable "CityatSpotRsrcBuildingNum" 0.f +infinityf
    let SpotRsrcBuildingNear = SpotRsrcBuildingNearSets |> fuzzySystem.CreateVariable "SpotRsrcBuildingNear" 0.f +infinityf
    let SpotMyUnitDist = MyUnitDistSets |> fuzzySystem.CreateVariable "SpotMyUnitDist" 0.f +infinityf
    let DeploySpotEnemDist = DeployDistEnemSets |> fuzzySystem.CreateVariable "DeploySpotEnemDist" 0.f +infinityf

    // DeployTask Common Output Variables

    let DeployUnittoCity = ResultSets |> fuzzySystem.CreateVariable "DeployUnittoCity" -1.000f 1.000f
    let DeployInteriorBuildingtoSpot = ResultSets |> fuzzySystem.CreateVariable "DeployInteriorBuildingtoSpot" -1.000f 1.000f
    let DeployTileResourceBuildingtoSpot = ResultSets |> fuzzySystem.CreateVariable "DeployTileResourceBuildingtoSpot" -1.000f 1.000f
    let DeployMilitaryBuildingtoSpot = ResultSets |> fuzzySystem.CreateVariable "DeployMilitaryBuildingtoSpot" -1.000f 1.000f
    let DeployCitytoPioneer = ResultSets |> fuzzySystem.CreateVariable "DeployCitytoPioneer" -1.000f 1.000f

    // UseSkillTask Common Input Variables

    let SkillEffect = SkillEffectTypeSets |> fuzzySystem.CreateVariable "SkillEffect" 0.0f 3.0f
    let SkillTarget = SkillTargetTypeSets |> fuzzySystem.CreateVariable "SkillTarget" 0.0f 2.0f
    let SkillTargetHP = HPSets |> fuzzySystem.CreateVariable "SkillTargetHP" 0.0f 1.0f
    let SkillTargetSpotDamage = SpotDamageSets |> fuzzySystem.CreateVariable "SkillTargetSpotDamage" 0.0f infinityf
    let SkillTargetSpotDeal = SpotDealSets |> fuzzySystem.CreateVariable "SkillTargetSpotDeal" 0.0f infinityf
    
    // UseSkillTask Common Output Variables

    let UseSKill = ResultSets |> fuzzySystem.CreateVariable "UseSkill" -1.0f 1.0f

    // Rules

    let RuleStringGlobalCommon = """
IF RemainingGold is RemainGoldVeryLow OR RemainingGold is RemainGoldLow THEN NeedFightingUnit is VeryLow
IF RemainingLabor is RemainLaborVeryLow OR RemainingLabor is RemainLaborLow THEN NeedFightingUnit is VeryLow
IF NeedCity is VeryLow THEN NeedPioneer is VeryLow
IF NeedCity is Low THEN NeedPioneer is Low
IF NeedCity is Medium THEN NeedPioneer is Medium
IF NeedCity is High THEN NeedPioneer is High
IF NeedCity is VeryHigh THEN NeedPioneer is VeryHigh
IF RemainingGold is RemainGoldVeryLow OR RemainingGold is RemainGoldLow THEN NeedCity is Low
IF RemainingGold is RemainGoldMedium THEN NeedCity is Medium
IF RemainingGold is RemainGoldHigh OR RemainingGold is RemainGoldVeryHigh THEN NeedCity is High
IF RemainingLabor is RemainLaborVeryLow OR RemainingLabor is RemainLaborLow THEN NeedCity is Low
IF RemainingLabor is RemainLaborMedium THEN NeedCity is Medium
IF RemainingLabor is RemainLaborHigh OR RemainingLabor is RemainLaborVeryHigh THEN NeedCity is High
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
IF Tech is TechMedium THEN NeedTech is Medium
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

    let RuleStringGlobalSuomiHwan = """
IF DeltaCityNum is CityHigh THEN NeedCity is VeryLow
IF DeltaCityNum is CityMedium THEN NeedCity is Low
IF DeltaCityNum is CityLow THEN NeedCity is Medium
IF DeltaFightingUnitNum is DeltaUnitHigh THEN NeedFightingUnit is VeryLow
IF DeltaFightingUnitNum is DeltaUnitMedium THEN NeedFightingUnit is Medium
IF DeltaFightingUnitNum is DeltaUnitLow THEN NeedFightingUnit is VeryHigh
IF EnemyFightingUnitNum is FightingUnitNumHigh THEN NeedMilitaryBuilding is High
IF EnemyFightingUnitNum is FightingUnitNumMedium THEN NeedMilitaryBuilding is Medium
IF EnemyFightingUnitNum is FightingUnitNumLow THEN NeedMilitaryBuilding is Low
"""

    let RuleStringGlobalOther = """
IF MyFightingUnitNum is FightingUnitNumHigh THEN NeedFightingUnit is VeryLow
IF MyFightingUnitNum is FightingUnitNumMedium THEN NeedFightingUnit is Low
IF MyFightingUnitNum is FightingUnitNumLow THEN NeedFightingUnit is High
IF MyCityNum is MyCityNumHigh THEN NeedCity is VeryLow
IF MyCityNum is MyCityNumMedium THEN NeedCity is Low
IF MyCityNum is MyCityNumLow THEN NeedCity is High
"""

    let RuleStringMoveTaskCommon = """
IF MyUnitHP is HPHigh THEN MoveUnittoSpot is High
IF MyUnitHP is HPLow THEN MoveUnittoSpot is VeryLow
IF EnemyUnitHP is HPHigh THEN MoveUnittoSpot is Medium
IF EnemyUnitHP is HPLow THEN MoveUnittoSpot is Low
IF EnemyCityHP is HPHigh THEN MoveUnittoSpot is High
IF EnemyCityHP is HPHigh THEN MoveUnittoSpot is VeryHigh
IF SpotDamage is DamageHigh THEN MoveUnittoSpot is VeryLow
IF SpotDamage is DamageMedium THEN MoveUnittoSpot is Low
IF SpotDamage is DamageLow THEN MoveUnittoSpot is Medium
IF SpotDeal is DealHigh THEN MoveUnittoSpot is High
IF SpotDeal is DealLow THEN MoveUnittoSpot is Low
IF SpotEnemDist is DistEnemMedium THEN MoveUnittoSpot is Low
IF SpotMyCityDist is DistMyCityMedium THEN MoveUnittoSpot is Low
IF SpotEnemDist is DistEnemLow AND MyUnit is FightingUnit AND MyUnitHP is HPHigh THEN MoveUnittoSpot is High
IF SpotEnemDist is DistEnemHigh AND MyUnit is FightingUnit AND MyUnitHP is HPLow THEN MoveUnittoSpot is High
IF SpotMyCityDist is DistMyCityLow AND MyUnit is FightingUnit AND MyUnitHP is HPLow THEN MoveUnittoSpot is High
IF SpotEnemDist is DistEnemHigh AND MyUnit is Pioneer THEN MoveUnittoSpot is High
IF SpotEnemDist is DistEnemLow AND MyUnit is Pioneer THEN MoveUnittoSpot is Low
IF SpotMyCityDist is DistMyCityHigh AND MyUnit is Pioneer THEN MoveUnittoSpot is High
IF SpotMyCityDist is DistMyCityLow AND MyUnit is Pioneer THEN MoveUnittoSpot is Low
"""

    let RuleStringMoveTaskOther = """
IF SpotOwner is Enemy OR SpotOwner is Ally THEN MoveUnittoSpot is VeryLow
IF SpotOwner is Me THEN MoveUnittoSpot is Medium
"""

    let RuleStringBuildTaskCommon = """
IF BuildingProdResource is Gold AND (Gold is GoldLow OR NeedGold is VeryHigh OR NeedGold is High) THEN BuildResourceBuilding is VeryHigh
IF BuildingProdResource is Gold AND (Gold is GoldMedium OR NeedGold is Medium) THEN BuildResourceBuilding is Medium
IF BuildingProdResource is Gold AND (Gold is GoldHigh OR NeedGold is Low OR NeedGold is VeryLow) THEN BuildResourceBuilding is Low
IF BuildingProdResource is Tech AND (Tech is TechLow OR NeedTech is VeryHigh OR NeedTech is High) THEN BuildResourceBuilding is VeryHigh
IF BuildingProdResource is Tech AND (Tech is TechMedium OR NeedTech is Medium) THEN BuildResourceBuilding is Medium
IF BuildingProdResource is Tech AND (Tech is TechHigh OR NeedTech is Low OR NeedTech is VeryLow) THEN BuildResourceBuilding is Low
IF BuildingProdResource is Labor AND (Labor is LaborLow OR NeedLabor is High OR NeedLabor is VeryHigh) THEN BuildResourceBuilding is High
IF BuildingProdResource is Labor AND (Labor is LaborMedium OR NeedLabor is Medium) THEN BuildResourceBuilding is Medium
IF BuildingProdResource is Labor AND (Labor is LaborHigh OR NeedLabor is Low OR NeedLabor is VeryLow) THEN BuildResourceBuilding is Low
IF BuildingProdResource is Happy AND (Happy is HappyLow OR NeedHappy is High OR NeedHappy is VeryHigh) THEN BuildResourceBuilding is High
IF BuildingProdResource is Happy AND (Happy is HappyMedium OR NeedHappy is Medium) THEN BuildResourceBuilding is Medium
IF BuildingProdResource is Happy AND (Happy is HappyHigh OR NeedHappy is Low OR NeedHappy is VeryLow) THEN BuildResourceBuilding is Low
"""

    let RuleStringDeployTaskCommon = """
IF DeploySpotEnemDist is DeployDistEnemLow AND MyUnit is FightingUnit THEN DeployUnittoCity is VeryHigh
IF RemainingGold is RemainGoldLow THEN DeployUnittoCity is Low
IF DeploySpotEnemDist is DeployDistEnemHigh AND MyUnit is Pioneer THEN DeployUnittoCity is VeryHigh
IF DeploySpotEnemDist is DeployDistEnemHigh THEN DeployInteriorBuildingtoSpot is VeryHigh
IF CityatSpotRsrcBuildingNum is RsrcBuildingNumHigh THEN DeployInteriorBuildingtoSpot is Low
IF DeploySpotEnemDist is DeployDistEnemLow THEN DeployInteriorBuildingtoSpot is VeryLow
IF CityatSpotRsrcBuildingNum is RsrcBuildingNumLow THEN DeployInteriorBuildingtoSpot is High
IF DeploySpotEnemDist is DeployDistEnemHigh THEN DeployTileResourceBuildingtoSpot is VeryHigh
IF DeploySpotEnemDist is DeployDistEnemLow THEN DeployTileResourceBuildingtoSpot is VeryLow
IF SpotRsrcBuildingNear is SpotRsrcBuildingNearHigh THEN DeployTileResourceBuildingtoSpot is Low
IF SpotRsrcBuildingNear is SpotRsrcBuildingNearLow THEN DeployTileResourceBuildingtoSpot is High
IF SpotMyUnitDist is MyUnitDistLow THEN DeployMilitaryBuildingtoSpot is High
IF DeploySpotEnemDist is DeployDistEnemLow THEN DeployMilitaryBuildingtoSpot is Low
IF NeedCity is High THEN DeployCitytoPioneer is High
IF NeedCity is Medium THEN DeployCitytoPioneer is High
IF NeedCity is Low THEN DeployCitytoPioneer is Medium
"""

    let RuleStringUseSkillCommon = """
IF SkillEffect is DamageEnemy AND SkillTargetHP is HPLow AND SkillTarget is Enemy THEN UseSkill is VeryHigh
IF SkillEffect is DamageEnemy AND SkillTargetHP is HPHigh AND SkillTarget is Enemy THEN UseSkill is High
IF SkillEffect is RestoreAP THEN UseSkill is VeryHigh
IF SkillEffect is RestoreHP AND SkillTargetHP is HPLow AND SkillTarget is Ally THEN UseSkill is VeryHigh
IF SkillEffect is RestoreHP AND SkillTargetHP is HPHigh  AND SkillTarget is Ally THEN UseSkill is Medium
IF SkillEffect is RestoreHP AND SkillTargetSpotDamage is SpotDamageHigh AND SkillTarget is Ally THEN UseSkill is High
IF SkillEffect is Buff AND SkillTargetSpotDamage is SpotDamageHigh AND SkillTarget is Ally THEN UseSkill is High
IF SkillEffect is Buff AND SkillTargetSpotDeal is SpotDealHigh AND SkillTarget is Ally THEN UseSkill is High
"""
