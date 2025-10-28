package com.company.assembleegameclient.util {
import com.company.assembleegameclient.engine3d.Model3D;
import com.company.assembleegameclient.map.GroundLibrary;
import com.company.assembleegameclient.map.RegionLibrary;
import com.company.assembleegameclient.objects.ObjectLibrary;
import com.company.assembleegameclient.objects.Wall;
import com.company.assembleegameclient.objects.particles.ParticleLibrary;
import com.company.assembleegameclient.parameters.Parameters;
import com.company.assembleegameclient.sound.SFX;
import com.company.assembleegameclient.sound.SoundEffectLibrary;
import com.company.assembleegameclient.ui.options.Options;
import com.company.util.AssetLibrary;

import flash.display.BitmapData;
import flash.filesystem.File;
import flash.filesystem.FileMode;
import flash.filesystem.FileStream;
import flash.net.FileReference;
import flash.net.URLLoader;
import flash.net.URLRequest;
import flash.utils.ByteArray;

import kabam.rotmg.assets.EmbeddedAssets;
import kabam.rotmg.assets.EmbeddedData;
import kabam.rotmg.emote.Emotes;

public class AssetLoader {

    public function AssetLoader() {
        super();
    }

    public function load():void {
        this.addImages();
        this.addAnimatedCharacters();
        this.addSoundEffects();
        this.parse3DModels();
        this.parseParticleEffects();
        this.parseGroundFiles();
        this.parseObjectFiles();
        this.parseRegionFiles();
        Parameters.load();
        Options.refreshCursor();
        Emotes.load();
    }

    private function addImages():void {
        AssetLibrary.addImageSet("lofiChar8x8", EmbeddedAssets.lofiCharEmbed_, 8, 8);
        AssetLibrary.addImageSet("lofiChar16x8", EmbeddedAssets.lofiCharEmbed_, 16, 8);
        AssetLibrary.addImageSet("lofiChar16x16", EmbeddedAssets.lofiCharEmbed_, 16, 16);
        AssetLibrary.addImageSet("lofiChar28x8", EmbeddedAssets.lofiChar2Embed_, 8, 8);
        AssetLibrary.addImageSet("lofiChar216x8", EmbeddedAssets.lofiChar2Embed_, 16, 8);
        AssetLibrary.addImageSet("lofiChar216x16", EmbeddedAssets.lofiChar2Embed_, 16, 16);
        AssetLibrary.addImageSet("lofiCharBig", EmbeddedAssets.lofiCharBigEmbed_, 16, 16);
        AssetLibrary.addImageSet("lofiEnvironment", EmbeddedAssets.lofiEnvironmentEmbed_, 8, 8);
        AssetLibrary.addImageSet("lofiEnvironment2", EmbeddedAssets.lofiEnvironment2Embed_, 8, 8);
        AssetLibrary.addImageSet("lofiEnvironment3", EmbeddedAssets.lofiEnvironment3Embed_, 8, 8);
        AssetLibrary.addImageSet("lofiInterface", EmbeddedAssets.lofiInterfaceEmbed_, 8, 8);
        AssetLibrary.addImageSet("redLootBag", EmbeddedAssets.redLootBagEmbed_, 8, 8);
        AssetLibrary.addImageSet("lofiInterfaceBig", EmbeddedAssets.lofiInterfaceBigEmbed_, 16, 16);
        AssetLibrary.addImageSet("lofiInterface2", EmbeddedAssets.lofiInterface2Embed_, 8, 8);
        AssetLibrary.addImageSet("lofiObj", EmbeddedAssets.lofiObjEmbed_, 8, 8);
        AssetLibrary.addImageSet("lofiObj2", EmbeddedAssets.lofiObj2Embed_, 8, 8);
        AssetLibrary.addImageSet("lofiObj3", EmbeddedAssets.lofiObj3Embed_, 8, 8);
        AssetLibrary.addImageSet("lofiObj4", EmbeddedAssets.lofiObj4Embed_, 8, 8);
        AssetLibrary.addImageSet("lofiObj5", EmbeddedAssets.lofiObj5Embed_, 8, 8);
        AssetLibrary.addImageSet("lofiObj5new", EmbeddedAssets.lofiObj5bEmbed_, 8, 8);
        AssetLibrary.addImageSet("lofiObj6", EmbeddedAssets.lofiObj6Embed_, 8, 8);
        AssetLibrary.addImageSet("lofiObjBig", EmbeddedAssets.lofiObjBigEmbed_, 16, 16);
        AssetLibrary.addImageSet("lofiObj40x40", EmbeddedAssets.lofiObj40x40Embed_, 40, 40);
        AssetLibrary.addImageSet("lofiProjs", EmbeddedAssets.lofiProjsEmbed_, 8, 8);
        AssetLibrary.addImageSet("lofiParticlesShocker", EmbeddedAssets.lofiParticlesShockerEmbed_, 16, 16);
        AssetLibrary.addImageSet("lofiParticlesBeam", EmbeddedAssets.lofiParticlesBeamEmbed_, 16, 32);
        AssetLibrary.addImageSet("lofiParticlesSkull", EmbeddedAssets.lofiParticlesSkullEmbed_, 16, 32);
        AssetLibrary.addImageSet("lofiParticlesElectric", EmbeddedAssets.lofiParticlesElectricEmbed_, 32, 32);
        AssetLibrary.addImageSet("lofiProjsBig", EmbeddedAssets.lofiProjsBigEmbed_, 16, 16);
        AssetLibrary.addImageSet("lofiParts", EmbeddedAssets.lofiPartsEmbed_, 8, 8);
        AssetLibrary.addImageSet("stars", EmbeddedAssets.starsEmbed_, 5, 5);
        AssetLibrary.addImageSet("textile4x4", EmbeddedAssets.textile4x4Embed_, 4, 4);
        AssetLibrary.addImageSet("textile5x5", EmbeddedAssets.textile5x5Embed_, 5, 5);
        AssetLibrary.addImageSet("textile9x9", EmbeddedAssets.textile9x9Embed_, 9, 9);
        AssetLibrary.addImageSet("textile10x10", EmbeddedAssets.textile10x10Embed_, 10, 10);
        AssetLibrary.addImageSet("inner_mask", EmbeddedAssets.innerMaskEmbed_, 4, 4);
        AssetLibrary.addImageSet("sides_mask", EmbeddedAssets.sidesMaskEmbed_, 4, 4);
        AssetLibrary.addImageSet("outer_mask", EmbeddedAssets.outerMaskEmbed_, 4, 4);
        AssetLibrary.addImageSet("innerP1_mask", EmbeddedAssets.innerP1MaskEmbed_, 4, 4);
        AssetLibrary.addImageSet("innerP2_mask", EmbeddedAssets.innerP2MaskEmbed_, 4, 4);
        AssetLibrary.addImageSet("invisible", null, 8, 8);
        AssetLibrary.addImageSet("d3LofiObjEmbed", EmbeddedAssets.d3LofiObjEmbed_, 8, 8);
        AssetLibrary.addImageSet("d3LofiObjEmbed16", EmbeddedAssets.d3LofiObjEmbed_, 16, 16);
        AssetLibrary.addImageSet("d3LofiObjBigEmbed", EmbeddedAssets.d3LofiObjBigEmbed_, 16, 16);
        AssetLibrary.addImageSet("d2LofiObjEmbed", EmbeddedAssets.d2LofiObjEmbed_, 8, 8);
        AssetLibrary.addImageSet("d2LofiObjBigEmbed", EmbeddedAssets.d2LofiObjBigEmbed_, 16, 16);
        AssetLibrary.addImageSet("d1lofiObjBig", EmbeddedAssets.d1LofiObjBigEmbed_, 16, 16);
        AssetLibrary.addImageSet("cursorsEmbed", EmbeddedAssets.cursorsEmbed_, 32, 32);
        AssetLibrary.addImageSet("mountainTempleObjects8x8", EmbeddedAssets.mountainTempleObjects8x8Embed_, 8, 8);
        AssetLibrary.addImageSet("mountainTempleObjects16x16", EmbeddedAssets.mountainTempleObjects16x16Embed_, 16, 16);
        AssetLibrary.addImageSet("oryxHordeObjects8x8", EmbeddedAssets.oryxHordeObjects8x8Embed_, 8, 8);
        AssetLibrary.addImageSet("oryxHordeObjects16x16", EmbeddedAssets.oryxHordeObjects16x16Embed_, 16, 16);
        AssetLibrary.addImageSet("battleOryxObjects8x8", EmbeddedAssets.battleOryxObjects8x8Embed_, 8, 8);
        AssetLibrary.addImageSet("battleOryxObjects16x16", EmbeddedAssets.battleOryxObjects16x16Embed_, 16, 16);
        AssetLibrary.addImageSet("santaWorkshopObjects8x8", EmbeddedAssets.santaWorkshopObjects8x8Embed_, 8, 8);
        AssetLibrary.addImageSet("santaWorkshopObjects16x16", EmbeddedAssets.santaWorkshopObjects16x16Embed_, 16, 16);
        AssetLibrary.addImageSet("parasiteDenObjects8x8", EmbeddedAssets.parasiteDenObjects8x8Embed_, 8, 8);
        AssetLibrary.addImageSet("parasiteDenObjects16x16", EmbeddedAssets.parasiteDenObjects16x16Embed_, 16, 16);
        AssetLibrary.addImageSet("stPatricksObjects8x8", EmbeddedAssets.stPatricksObjects8x8Embed_, 8, 8);
        AssetLibrary.addImageSet("stPatricksObjects16x16", EmbeddedAssets.stPatricksObjects16x16Embed_, 16, 16);
        AssetLibrary.addImageSet("buffedBunnyObjects8x8", EmbeddedAssets.buffedBunnyObjects8x8Embed_, 8, 8);
        AssetLibrary.addImageSet("buffedBunnyObjects16x16", EmbeddedAssets.buffedBunnyObjects16x16Embed_, 16, 16);
        AssetLibrary.addImageSet("theMachineObjects8x8", EmbeddedAssets.theMachineObjects8x8Embed_, 8, 8);
        AssetLibrary.addImageSet("theMachineObjects16x16", EmbeddedAssets.theMachineObjects16x16Embed_, 16, 16);
        AssetLibrary.addImageSet("SakuraEnvironment16x16", EmbeddedAssets.SakuraEnvironment16x16Embed_, 16, 16);
        AssetLibrary.addImageSet("SakuraEnvironment8x8", EmbeddedAssets.SakuraEnvironment8x8Embed_, 8, 8);
        AssetLibrary.addImageSet("HanamiParts", EmbeddedAssets.HanamiParts8x8Embed_, 8, 8);
        AssetLibrary.addImageSet("summerNexusObjects8x8", EmbeddedAssets.summerNexusObjects8x8Embed_, 8, 8);
        AssetLibrary.addImageSet("summerNexusObjects16x16", EmbeddedAssets.summerNexusObjects16x16Embed_, 16, 16);
        AssetLibrary.addImageSet("autumnNexusObjects8x8", EmbeddedAssets.autumnNexusObjects8x8Embed_, 8, 8);
        AssetLibrary.addImageSet("autumnNexusObjects16x16", EmbeddedAssets.autumnNexusObjects16x16Embed_, 16, 16);
        AssetLibrary.addImageSet("xmasNexusObjects8x8", EmbeddedAssets.xmasNexusObjects8x8Embed_, 8, 8);
        AssetLibrary.addImageSet("xmasNexusObjects16x16", EmbeddedAssets.xmasNexusObjects16x16Embed_, 16, 16);
        AssetLibrary.addImageSet("epicHiveObjects8x8", EmbeddedAssets.epicHiveObjects8x8Embed_, 8, 8);
        AssetLibrary.addImageSet("epicHiveObjects16x16", EmbeddedAssets.epicHiveObjects16x16Embed_, 16, 16);
        AssetLibrary.addImageSet("lostHallsObjects8x8", EmbeddedAssets.lostHallsObjects8x8Embed_, 8, 8);
        AssetLibrary.addImageSet("lostHallsObjects16x16", EmbeddedAssets.lostHallsObjects16x16Embed_, 16, 16);
        AssetLibrary.addImageSet("cnidarianReefObjects8x8", EmbeddedAssets.cnidarianReefObjects8x8Embed_, 8, 8);
        AssetLibrary.addImageSet("cnidarianReefObjects16x16", EmbeddedAssets.cnidarianReefObjects16x16Embed_, 16, 16);
        AssetLibrary.addImageSet("magicWoodsObjects8x8", EmbeddedAssets.magicWoodsObjects8x8Embed_, 8, 8);
        AssetLibrary.addImageSet("magicWoodsObjects16x16", EmbeddedAssets.magicWoodsObjects16x16Embed_, 16, 16);
        AssetLibrary.addImageSet("secludedThicketObjects8x8", EmbeddedAssets.secludedThicketObjects8x8Embed_, 8, 8);
        AssetLibrary.addImageSet("secludedThicketObjects16x16", EmbeddedAssets.secludedThicketObjects16x16Embed_, 16, 16);
        AssetLibrary.addImageSet("lofiGravestone8x8", EmbeddedAssets.lofiGravestoneEmbed_, 8, 8);
        AssetLibrary.addImageSet("lofiGravestone16x8", EmbeddedAssets.lofiGravestoneEmbed_, 16, 8);
        AssetLibrary.addImageSet("lofiGravestone16x16", EmbeddedAssets.lofiGravestoneEmbed_, 16, 16);
        AssetLibrary.addImageSet("cursedLibraryObjects8x8", EmbeddedAssets.cursedLibraryObjects8x8Embed_, 8, 8);
        AssetLibrary.addImageSet("cursedLibraryObjects16x16", EmbeddedAssets.cursedLibraryObjects16x16Embed_, 16, 16);
        AssetLibrary.addImageSet("lairOfDraconisObjects8x8", EmbeddedAssets.lairOfDraconisObjects8x8Embed_, 8, 8);
        AssetLibrary.addImageSet("lairOfDraconisObjects16x16", EmbeddedAssets.lairOfDraconisObjects16x16Embed_, 16, 16);
        AssetLibrary.addImageSet("lairOfShaitanObjects8x8", EmbeddedAssets.lairOfShaitanObjects8x8Embed_, 8, 8);
        AssetLibrary.addImageSet("lairOfShaitanObjects16x16", EmbeddedAssets.lairOfShaitanObjects16x16Embed_, 16, 16);
        AssetLibrary.addImageSet("alienInvasionObjects8x8", EmbeddedAssets.alienInvasionObjects8x8Embed_, 8, 8);
        AssetLibrary.addImageSet("alienInvasionObjects16x16", EmbeddedAssets.alienInvasionObjects16x16Embed_, 16, 16);
        AssetLibrary.addImageSet("crystalCaveObjects8x8", EmbeddedAssets.crystalCaveObjects8x8Embed_, 8, 8);
        AssetLibrary.addImageSet("crystalCaveObjects16x16", EmbeddedAssets.crystalCaveObjects16x16Embed_, 16, 16);
        AssetLibrary.addImageSet("fungalCavernObjects8x8", EmbeddedAssets.fungalCavernObjects8x8Embed_, 8, 8);
        AssetLibrary.addImageSet("fungalCavernObjects16x16", EmbeddedAssets.fungalCavernObjects16x16Embed_, 16, 16);
        AssetLibrary.addImageSet("innerWorkingsObjects8x8", EmbeddedAssets.innerWorkingsObjects8x8Embed_, 8, 8);
        AssetLibrary.addImageSet("innerWorkingsObjects16x16", EmbeddedAssets.innerWorkingsObjects16x16Embed_, 16, 16);
        AssetLibrary.addImageSet("lofiparticlesMusicNotes", EmbeddedAssets.lofi_particlesMusicNotesEmbed_, 16, 16);
        AssetLibrary.addImageSet("oryxSanctuaryObjects8x8", EmbeddedAssets.oryxSanctuaryObjects8x8Embed_, 8, 8);
        AssetLibrary.addImageSet("oryxSanctuaryObjects16x16", EmbeddedAssets.oryxSanctuaryObjects16x16Embed_, 16, 16);
        AssetLibrary.addImageSet("oryxSanctuaryObjects32x32", EmbeddedAssets.oryxSanctuaryObjects32x32Embed_, 32, 32);
        AssetLibrary.addImageSet("lofiParticlesHolyBeam", EmbeddedAssets.lofiParticlesHolyBeamEmbed_, 16, 32);
        AssetLibrary.addImageSet("lofiParticlesMeteor", EmbeddedAssets.lofiParticlesMeteorEmbed_, 32, 32);
        AssetLibrary.addImageSet("lofiParticlesTelegraph", EmbeddedAssets.lofiParticlesTelegraphEmbed_, 32, 32);
        AssetLibrary.addImageSet("archbishopObjects8x8", EmbeddedAssets.archbishopObjects8x8Embed_, 8, 8);
        AssetLibrary.addImageSet("archbishopObjects16x16", EmbeddedAssets.archbishopObjects16x16Embed_, 16, 16);
        AssetLibrary.addImageSet("archbishopObjects64x64", EmbeddedAssets.archbishopObjects64x64Embed_, 64, 64);
        AssetLibrary.addImageSet("ancientRuinsObjects8x8", EmbeddedAssets.ancientRuinsObjects8x8Embed_, 8, 8);
        AssetLibrary.addImageSet("ancientRuinsObjects16x16", EmbeddedAssets.ancientRuinsObjects16x16Embed_, 16, 16);

        AssetLibrary.addImageSet("cursorsEmbed", EmbeddedAssets.cursorsEmbed_, 32, 32);
        AssetLibrary.addImageSet("emotes", EmbeddedAssets.emotesEmbed_, 16, 16);
    }

    private function addAnimatedCharacters():void {
        AnimatedChars.add("chars8x8rBeach", EmbeddedAssets.chars8x8rBeachEmbed_, null, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("chars8x8dBeach", EmbeddedAssets.chars8x8dBeachEmbed_, null, 8, 8, 56, 8, AnimatedChar.DOWN);
        AnimatedChars.add("chars8x8rLow1", EmbeddedAssets.chars8x8rLow1Embed_, null, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("chars8x8rLow2", EmbeddedAssets.chars8x8rLow2Embed_, null, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("chars8x8rMid", EmbeddedAssets.chars8x8rMidEmbed_, null, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("chars8x8rMid2", EmbeddedAssets.chars8x8rMid2Embed_, null, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("chars8x8rHigh", EmbeddedAssets.chars8x8rHighEmbed_, null, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("chars8x8rHero1", EmbeddedAssets.chars8x8rHero1Embed_, null, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("chars8x8rHero2", EmbeddedAssets.chars8x8rHero2Embed_, null, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("chars8x8dHero1", EmbeddedAssets.chars8x8dHero1Embed_, null, 8, 8, 56, 8, AnimatedChar.DOWN);
        AnimatedChars.add("chars16x16dMountains1", EmbeddedAssets.chars16x16dMountains1Embed_, null, 16, 16, 112, 16, AnimatedChar.DOWN);
        AnimatedChars.add("chars16x16dMountains2", EmbeddedAssets.chars16x16dMountains2Embed_, null, 16, 16, 112, 16, AnimatedChar.DOWN);
        AnimatedChars.add("chars8x8dEncounters", EmbeddedAssets.chars8x8dEncountersEmbed_, null, 8, 8, 56, 8, AnimatedChar.DOWN);
        AnimatedChars.add("chars8x8rEncounters", EmbeddedAssets.chars8x8rEncountersEmbed_, null, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("chars16x8dEncounters", EmbeddedAssets.chars16x8dEncountersEmbed_, null, 16, 8, 112, 8, AnimatedChar.DOWN);
        AnimatedChars.add("chars16x8rEncounters", EmbeddedAssets.chars16x8rEncountersEmbed_, null, 16, 8, 112, 8, AnimatedChar.DOWN);
        AnimatedChars.add("chars16x16dEncounters", EmbeddedAssets.chars16x16dEncountersEmbed_, null, 16, 16, 112, 16, AnimatedChar.DOWN);
        AnimatedChars.add("chars16x16dEncounters2", EmbeddedAssets.chars16x16dEncounters2Embed_, null, 16, 16, 112, 16, AnimatedChar.DOWN);
        AnimatedChars.add("chars16x16rEncounters", EmbeddedAssets.chars16x16rEncountersEmbed_, null, 16, 16, 112, 16, AnimatedChar.RIGHT);
        AnimatedChars.add("d3Chars8x8rEmbed", EmbeddedAssets.d3Chars8x8rEmbed_, null, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("d3Chars16x16rEmbed", EmbeddedAssets.d3Chars16x16rEmbed_, null, 16, 16, 112, 16, AnimatedChar.RIGHT);
        AnimatedChars.add("d2Chars16x16rEmbed", EmbeddedAssets.d2Chars16x16rEmbed_, null, 16, 16, 112, 16, AnimatedChar.RIGHT);
        AnimatedChars.add("players", EmbeddedAssets.playersEmbed_, EmbeddedAssets.playersMaskEmbed_, 8, 8, 56, 24, AnimatedChar.RIGHT);
        AnimatedChars.add("playerskins", EmbeddedAssets.playersSkinsEmbed_, EmbeddedAssets.playersSkinsMaskEmbed_, 8, 8, 56, 24, AnimatedChar.RIGHT);
        AnimatedChars.add("chars8x8rPets1", EmbeddedAssets.chars8x8rPets1Embed_, EmbeddedAssets.chars8x8rPets1MaskEmbed_, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("chars8x8rPets2", EmbeddedAssets.chars8x8rPets2Embed_, EmbeddedAssets.chars8x8rPets1MaskEmbed_, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("petsDivine", EmbeddedAssets.petsDivineEmbed_, null, 16, 16, 112, 16, AnimatedChar.RIGHT);
        AnimatedChars.add("playerskins16", EmbeddedAssets.playersSkins16Embed_, EmbeddedAssets.playersSkins16MaskEmbed_, 16, 16, 112, 48, AnimatedChar.RIGHT);
        AnimatedChars.add("d1chars16x16r", EmbeddedAssets.d1Chars16x16rEmbed_, null, 16, 16, 112, 16, AnimatedChar.RIGHT);
        AnimatedChars.add("parasiteDenChars8x8", EmbeddedAssets.parasiteDenChars8x8Embed_, null, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("parasiteDenChars16x16", EmbeddedAssets.parasiteDenChars16x16Embed_, null, 16, 16, 112, 16, AnimatedChar.RIGHT);
        AnimatedChars.add("stPatricksChars8x8", EmbeddedAssets.stPatricksChars8x8Embed_, null, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("stPatricksChars16x16", EmbeddedAssets.stPatricksChars16x16Embed_, null, 16, 16, 112, 16, AnimatedChar.RIGHT);
        AnimatedChars.add("buffedBunnyChars16x16", EmbeddedAssets.buffedBunnyChars16x16Embed_, null, 16, 16, 112, 16, AnimatedChar.RIGHT);
        AnimatedChars.add("theMachineChars8x8", EmbeddedAssets.theMachineChars8x8Embed_, null, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("theMachineChars16x16", EmbeddedAssets.theMachineChars16x16Embed_, null, 16, 16, 112, 16, AnimatedChar.RIGHT);
        AnimatedChars.add("theGoldenArcher", EmbeddedAssets.theGoldenArcherEmbed_, EmbeddedAssets.theGoldenArcherMaskEmbed_, 8, 8, 56, 24, AnimatedChar.RIGHT);
        AnimatedChars.add("mountainTempleChars8x8", EmbeddedAssets.mountainTempleChars8x8Embed_, null, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("mountainTempleChars16x16", EmbeddedAssets.mountainTempleChars16x16Embed_, null, 16, 16, 112, 16, AnimatedChar.RIGHT);
        AnimatedChars.add("oryxHordeChars8x8", EmbeddedAssets.oryxHordeChars8x8Embed_, null, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("oryxHordeChars16x16", EmbeddedAssets.oryxHordeChars16x16Embed_, null, 16, 16, 112, 16, AnimatedChar.RIGHT);
        AnimatedChars.add("battleOryxChars8x8", EmbeddedAssets.battleOryxChars8x8Embed_, null, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("battleOryxChars16x16", EmbeddedAssets.battleOryxChars16x16Embed_, null, 16, 16, 112, 16, AnimatedChar.RIGHT);
        AnimatedChars.add("santaWorkshopChars8x8", EmbeddedAssets.santaWorkshopChars8x8Embed_, null, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("santaWorkshopChars16x16", EmbeddedAssets.santaWorkshopChars16x16Embed_, null, 16, 16, 112, 16, AnimatedChar.RIGHT);
        AnimatedChars.add("Hanami8x8chars", EmbeddedAssets.Hanami8x8charsEmbed_, null, 8, 8, 64, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("summerNexusChars8x8", EmbeddedAssets.summerNexusChars8x8Embed_, null, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("summerNexusChars16x16", EmbeddedAssets.summerNexusChars16x16Embed_, null, 16, 16, 112, 16, AnimatedChar.RIGHT);
        AnimatedChars.add("autumnNexusChars16x16", EmbeddedAssets.autumnNexusChars16x16Embed_, null, 16, 16, 112, 16, AnimatedChar.RIGHT);
        AnimatedChars.add("autumnNexusChars8x8", EmbeddedAssets.autumnNexusChars8x8Embed_, null, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("xmasNexusChars8x8", EmbeddedAssets.xmasNexusChars8x8Embed_, null, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("xmasNexusChars16x16", EmbeddedAssets.xmasNexusChars16x16Embed_, null, 16, 16, 112, 16, AnimatedChar.RIGHT);
        AnimatedChars.add("epicHiveChars8x8", EmbeddedAssets.epicHiveChars8x8Embed_, null, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("epicHiveChars16x16", EmbeddedAssets.epicHiveChars16x16Embed_, null, 16, 16, 112, 16, AnimatedChar.RIGHT);
        AnimatedChars.add("lostHallsChars16x16", EmbeddedAssets.lostHallsChars16x16Embed_, null, 16, 16, 112, 16, AnimatedChar.RIGHT);
        AnimatedChars.add("lostHallsChars8x8", EmbeddedAssets.lostHallsChars8x8Embed_, null, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("magicWoodsChars8x8", EmbeddedAssets.magicWoodsChars8x8Embed_, null, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("magicWoodsChars16x16", EmbeddedAssets.magicWoodsChars16x16Embed_, null, 16, 16, 112, 16, AnimatedChar.RIGHT);
        AnimatedChars.add("secludedThicketChars8x8", EmbeddedAssets.secludedThicketChars8x8Embed_, null, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("secludedThicketChars16x16", EmbeddedAssets.secludedThicketChars16x16Embed_, null, 16, 16, 112, 16, AnimatedChar.RIGHT);
        AnimatedChars.add("cursedLibraryChars8x8", EmbeddedAssets.cursedLibraryChars8x8Embed_, null, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("cursedLibraryChars16x16", EmbeddedAssets.cursedLibraryChars16x16Embed_, null, 16, 16, 112, 16, AnimatedChar.RIGHT);
        AnimatedChars.add("cursedLibraryCharsAvalon16x16", EmbeddedAssets.cursedLibraryCharsAvalon16x16Embed_, null, 16, 16, 112, 48, AnimatedChar.RIGHT);
        AnimatedChars.add("lairOfDraconisChars8x8", EmbeddedAssets.lairOfDraconisChars8x8Embed_, null, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("lairOfDraconisChars16x16", EmbeddedAssets.lairOfDraconisChars16x16Embed_, null, 16, 16, 112, 16, AnimatedChar.RIGHT);
        AnimatedChars.add("lairOfShaitanChars16x16", EmbeddedAssets.lairOfShaitanChars16x16Embed_, null, 16, 16, 112, 16, AnimatedChar.RIGHT);
        AnimatedChars.add("alienInvasionChars8x8", EmbeddedAssets.alienInvasionChars8x8Embed_, null, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("alienInvasionChars16x16", EmbeddedAssets.alienInvasionChars16x16Embed_, null, 16, 16, 112, 16, AnimatedChar.RIGHT);
        AnimatedChars.add("fungalCavernChars8x8", EmbeddedAssets.fungalCavernChars8x8Embed_, null, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("fungalCavernChars16x16", EmbeddedAssets.fungalCavernChars16x16Embed_, null, 16, 16, 112, 16, AnimatedChar.RIGHT);
        AnimatedChars.add("crystalCaveChars8x8", EmbeddedAssets.crystalCaveChars8x8Embed_, null, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("crystalCaveChars16x16", EmbeddedAssets.crystalCaveChars16x16Embed_, null, 16, 16, 112, 16, AnimatedChar.RIGHT);
        AnimatedChars.add("innerWorkingsChars8x8", EmbeddedAssets.innerWorkingsChars8x8Embed_, null, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("innerWorkingsChars16x16", EmbeddedAssets.innerWorkingsChars16x16Embed_, null, 16, 16, 112, 16, AnimatedChar.RIGHT);
        AnimatedChars.add("oryxSanctuaryChars8x8", EmbeddedAssets.oryxSanctuaryChars8x8Embed_, null, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("oryxSanctuaryChars16x16", EmbeddedAssets.oryxSanctuaryChars16x16Embed_, null, 16, 16, 112, 16, AnimatedChar.RIGHT);
        AnimatedChars.add("oryxSanctuaryChars32x32", EmbeddedAssets.oryxSanctuaryChars32x32Embed_, null, 32, 32, 224, 32, AnimatedChar.RIGHT);
        AnimatedChars.add("archbishopChars8x8", EmbeddedAssets.archbishopChars8x8Embed_, null, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("archbishopChars16x16", EmbeddedAssets.archbishopChars16x16Embed_, null, 16, 16, 112, 16, AnimatedChar.RIGHT);
        AnimatedChars.add("ancientRuinsChars8x8", EmbeddedAssets.ancientRuinsChars8x8Embed_, null, 8, 8, 56, 8, AnimatedChar.RIGHT);
        AnimatedChars.add("ancientRuinsChars16x16", EmbeddedAssets.ancientRuinsChars16x16Embed_, null, 16, 16, 112, 16, AnimatedChar.RIGHT);
    }

    private function addSoundEffects():void {
        SoundEffectLibrary.load("button_click");
        SoundEffectLibrary.load("death_screen");
        SoundEffectLibrary.load("enter_realm");
        SoundEffectLibrary.load("error");
        SoundEffectLibrary.load("inventory_move_item");
        SoundEffectLibrary.load("level_up");
        SoundEffectLibrary.load("loot_appears");
        SoundEffectLibrary.load("no_mana");
        SoundEffectLibrary.load("use_key");
        SoundEffectLibrary.load("use_potion");
    }

    private function parse3DModels():void {
        var name:* = null;
        var ba:ByteArray = null;
        var model:String = null;
        for (name in EmbeddedAssets.models_) {
            ba = EmbeddedAssets.models_[name];
            model = ba.readUTFBytes(ba.length);
            Model3D.parse3DOBJ(name, ba);
            Model3D.parseFromOBJ(name, model);
        }
    }

    private function parseParticleEffects():void {
        var xml:XML = XML(new EmbeddedAssets.particlesEmbed());
        ParticleLibrary.parseFromXML(xml);
    }

    private function parseGroundFiles():void {
        var groundObj:* = undefined;
        for each(groundObj in EmbeddedData.groundFiles) {
            GroundLibrary.parseFromXML(XML(groundObj));
        }
    }

    private function parseObjectFiles():void {
        var objectObj:* = undefined;
        for each(objectObj in EmbeddedData.objectFiles) {
            ObjectLibrary.parseFromXML(XML(objectObj));
        }
    }

    private function parseRegionFiles():void {
        var regionXML:* = undefined;
        for each(regionXML in EmbeddedData.regionFiles) {
            RegionLibrary.parseFromXML(XML(regionXML));
        }
    }
}
}
