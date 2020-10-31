# Konata

## 目標
 實現一個新的qqbot框架.

## TODOs
 - [x] 實現 ECDH 密鈅交換算法 基於橢圓曲綫 secp192k1
 - [x] 修復 TEA(TinyEncryptionAlogrithm) 16輪加密算法
 - [x] 打包TLV 製作 wtlogin 封包
 - [x] 驗證圖形滑塊
 - [x] 驗證SMS
 - [ ] 驗證設備鎖
 - [ ] OnlinePush
 - [ ] HeartBeat
 - [x] WtLogin.login
 - [ ] WtLogin.trans_emp
 - [x] StatSvc.register
 - [ ] StatSvc.GetOnlineStatus
 - [ ] StatSvc.SetStatusFromClient
 - [ ] StatSvc.SimpleGet
 - [ ] OidbSvc.0x480_9
 - [ ] OidbSvc.0x4ff_9
 - [ ] OidbSvc.0x55c_1 (setAdmin)
 - [x] OidbSvc.0x570_8 (mute)
 - [ ] OidbSvc.0x5eb_22
 - [ ] OidbSvc.0x6d6_2 (groupFile)
 - [x] OidbSvc.0x88d_0 (groupInfo)
 - [ ] OidbSvc.0x89a_0 (groupSetting)
 - [ ] OidbSvc.0x8a0_0 (kick)
 - [ ] OidbSvc.0x8fc_2 (setTitle)
 - [ ] OidbSvc.0xdc9
 - [ ] ~OidbSvc.oidb_0x758 (invite)~
 - [ ] OidbSvc.oidb_0xd82
 - [x] OidbSvc.0x899_0 (memberInfo)
 - [ ] ProfileService.GroupMngReq
 - [ ] ProfileService.GetSimpleInfo
 - [ ] ProfileService.Pb.ReqSystemMsgNew.Friend
 - [ ] ProfileService.Pb.ReqSystemMsgNew.Group
 - [ ] ProfileService.Pb.ReqSystemMsgAction.Friend
 - [ ] ProfileService.Pb.ReqSystemMsgAction.Group
 - [x] OnlinePush.PbPushGroupMsg
 - [ ] OnlinePush.PbPushDisMsg
 - [ ] OnlinePush.ReqPush
 - [ ] OnlinePush.PbPushTransMsg
 - [ ] OnlinePush.PbC2CMsgSync
 - [ ] OnlinePush.SidTicketExpired
 - [ ] MessageSvc.PushNotify
 - [ ] MessageSvc.RequestPushStatus
 - [ ] MessageSvc.PushReaded
 - [ ] MessageSvc.PushForceOffline
 - [ ] StatSvc.ReqMSFOffline
 - [ ] StatSvc.SvcReqMSFLoginNotify
 - [ ] ConfigPushSvc.PushReq
 - [ ] ConfigPushSvc.PushDomain
 - [ ] QualityTest.PushList
 - [ ] [更多](../../projects/1)...
