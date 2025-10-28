package kabam.rotmg.ui.view {
import com.company.assembleegameclient.screens.AccountScreen;
import com.company.assembleegameclient.screens.CharacterSelectionAndNewsScreen;
import com.company.assembleegameclient.ui.tooltip.ToolTip;

import kabam.rotmg.account.core.Account;
import kabam.rotmg.account.core.view.AccountInfoView;
import kabam.rotmg.account.web.view.WebAccountInfoView;
import kabam.rotmg.core.model.PlayerModel;
import kabam.rotmg.core.model.ScreenModel;
import kabam.rotmg.core.signals.SetScreenWithValidDataSignal;

import robotlegs.bender.bundles.mvcs.Mediator;

public class AccountScreenMediator extends Mediator {

    [Inject]
    public var view:AccountScreen;

    [Inject]
    public var account:Account;

    [Inject]
    public var playerModel:PlayerModel;

    [Inject]
    public var setScreenWithValidData:SetScreenWithValidDataSignal;

    [Inject]
    public var screenModel:ScreenModel;

    public function AccountScreenMediator() {
        super();
    }

    override public function initialize():void {
        this.view.setRank(this.playerModel.getNumStars(), this.playerModel.getAccountRank());
        this.view.setGuild(this.playerModel.getGuildName(), this.playerModel.getGuildRank());
        this.view.setAccountInfo(this.getInfoView());
        this.view.reloadClicked.add(this.onReload);
    }

    private function getInfoView():AccountInfoView {
        var view:AccountInfoView = new WebAccountInfoView();
        return view;
    }

    private function onReload():void {
        this.playerModel.reloadData = true;
        this.setScreenWithValidData.dispatch(new this.screenModel.currentType());
    }
}
}
