using Lean.Pool;
using UnityEngine;

public class FxController : SingletonMonoBehaviour<FxController>
{
    [SerializeField] private GameObject _blockDestroyFXWood;
    [SerializeField] private GameObject _blockDestroyFXStone;
    [SerializeField] private GameObject _blockDestroyFXGlass;
    [SerializeField] private GameObject _blockDestroyFXExplosive;

    [SerializeField] private GameObject _pickUpScoreUpCollectFX;

    [SerializeField] private BlockScalerFx _blockScalerPrefab;
    [SerializeField] private FlyingMessage _flyingMessagePrefab;

    [SerializeField] private float _fxDestroyTime = 2f;
    
    public void PlayBlockHitFX(Block block)
    {
        LeanPool.Spawn(_blockScalerPrefab).SetTarget(block.transform);
        
        AudioController.Instance.PlaySfx(SfxType.BlockHit);
    }

    public void PlayBlockDestroyFX(Block block)
    {
        GameObject fxPrefab = block.Type switch
        {
            BlockType.Wood => _blockDestroyFXWood,
            BlockType.Stone => _blockDestroyFXStone,
            BlockType.Explosive => _blockDestroyFXExplosive,
            _ => _blockDestroyFXGlass
        };

        Vector3 position = block.transform.position;
        
        GameObject fx = LeanPool.Spawn(fxPrefab, position, Quaternion.identity);
        LeanPool.Despawn(fx, _fxDestroyTime);

        LeanPool.Spawn(_flyingMessagePrefab)
            .Init(position, $"+{block.ScoreForDestroy}", true);

        AudioController.Instance.PlaySfx(SfxType.BlockDestroy);
    }

    public void PlayPickUpScoreFX(PickUpScore pScore)
    {
        FlyingMessage fMsg = LeanPool.Spawn(_flyingMessagePrefab);
        Vector3 position = pScore.transform.position;

        if(pScore.Score > 0)
        {
            fMsg.Init(position, $"+{pScore.Score}", true);
            
            GameObject fx = LeanPool.Spawn(_pickUpScoreUpCollectFX, position, Quaternion.identity);
            LeanPool.Despawn(fx, _fxDestroyTime);

            AudioController.Instance.PlaySfx(SfxType.PickUpScoreUpCollect);
        }
        else
        {
            fMsg.Init(position, $"{pScore.Score}", false);

            AudioController.Instance.PlaySfx(SfxType.PickUpNegativeCollect);
        }
    }

    public void PlayPickUpSpeedFX(PickUpSpeed pSpeed)
    {
        FlyingMessage fm = LeanPool.Spawn(_flyingMessagePrefab);

        if(pSpeed.SpeedFactor > 1)
        {
            fm.Init(pSpeed.transform.position, "Speed Up", false);
        }
        else
        {
            fm.Init(pSpeed.transform.position, "Speed Down", true);
        }
    }

    public void PlayPickUpLifeFX(PickUpLifeUp pLifeUp)
    {
        Vector3 position = pLifeUp.transform.position;
        
        LeanPool.Spawn(_flyingMessagePrefab)
            .Init(position, "+1 Life", true);

        GameObject fx = LeanPool.Spawn(_pickUpScoreUpCollectFX, position, Quaternion.identity);
        LeanPool.Despawn(fx, _fxDestroyTime);

        AudioController.Instance.PlaySfx(SfxType.PickUpLifeUpCollect);
    }

    public void PlayPickUpPadWidthFX(PickUpPadWidth pPadWidth)
    {
        SfxType sfxType = (pPadWidth.WidthFactor > 1) ? SfxType.PickUpCollect : SfxType.PickUpNegativeCollect;
        
        AudioController.Instance.PlaySfx(sfxType);
    }

    public void PlayPickUpStickyFX()
    {
        //
        AudioController.Instance.PlaySfx(SfxType.PickUpCollect);
    }

    public void PlayPickUpAddBallsFX()
    {
        //
        AudioController.Instance.PlaySfx(SfxType.PickUpCollect);
    }

    public void PlayPickUpBallSizeFX()
    {
        //
        AudioController.Instance.PlaySfx(SfxType.PickUpCollect);
    }
}
