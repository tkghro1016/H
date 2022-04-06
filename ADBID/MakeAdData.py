from GetAdData import GetAPIMethod
import pandas as pd
import numpy as np

class MakeAPIMethod:
    def __init__(self, base_url, api_key, secret_key, customer_id):
        self.BASE_URL = base_url
        self.API_KEY = api_key
        self.SECRET_KEY = secret_key
        self.CUSTOMER_ID = customer_id

    def make_campaign_list(self):
        raw_cmp = GetAPIMethod.get_campaign_list(self.BASE_URL, self.API_KEY, self.SECRET_KEY, self.CUSTOMER_ID)
        cmp_data = []

        for eachCmpData in raw_cmp:
            tmp = {}
            tmp['nccCampaignId'] = eachCmpData['nccCampaignId']
            tmp['cmpName'] = eachCmpData['name']
            tmp['campaignTp'] = eachCmpData['campaignTp']
            cmp_data.append(tmp)

        return cmp_data

    def make_adgroup_list(self):
        raw_grp = GetAPIMethod.get_adgroup_list(self.BASE_URL, self.API_KEY, self.SECRET_KEY, self.CUSTOMER_ID)
        grp_data = []

        for eachGrpData in raw_grp:
            tmp = {}
            tmp['nccAdgroupId'] = eachGrpData['nccAdgroupId']
            tmp['grpName'] = eachGrpData['name']
            tmp['nccCampaignId'] = eachGrpData['nccCampaignId']
            grp_data.append(tmp)

        return grp_data

    def merge_cmp_and_grp_list(self, get_cmp_data, get_grp_data):
        for eachGrpData in get_grp_data:
            for eachCmpData in get_cmp_data:
                if (eachCmpData['nccCampaignId'] == eachGrpData['nccCampaignId']):
                    eachGrpData['cmpName'] = eachCmpData['cmpName']
                    eachGrpData['campaignTp'] = eachCmpData['campaignTp']

        return get_grp_data

    def make_adkeyword_list(self, grp_ids):
        #################################
        # Keyword Data 호출 부 (keywords / Ads 따로 호출)
        #################################
        kwd_data = []
        ad_data = []

        for eachGrpData in grp_ids:
            if (eachGrpData['campaignTp'] == 'WEB_SITE'):
                raw_kwd = GetAPIMethod.get_adkeyword_list(self.BASE_URL, self.API_KEY, self.SECRET_KEY, self.CUSTOMER_ID, eachGrpData['nccAdgroupId'])

                for eachKwdData in raw_kwd:
                    tmp = {}
                    tmp['id'] = eachKwdData['nccKeywordId']
                    tmp['nccAdgroupId'] = eachKwdData['nccAdgroupId']
                    tmp['type'] = 'easter_egg'
                    tmp['kwdName'] = eachKwdData['keyword']
                    tmp['bidAmt'] = eachKwdData['bidAmt']

                    kwd_data.append(tmp)
            elif (eachGrpData['campaignTp'] == 'SHOPPING'):
                raw_ad = GetAPIMethod.get_ad_list(self.BASE_URL, self.API_KEY, self.SECRET_KEY, self.CUSTOMER_ID, eachGrpData['nccAdgroupId'])

                for eachAdData in raw_ad:
                    tmp = {}
                    tmp['id'] = eachAdData['nccAdId']
                    tmp['nccAdgroupId'] = eachAdData['nccAdgroupId']
                    tmp['type'] = eachAdData['type']
                    tmp['kwdName'] = eachAdData['referenceData']['productTitle']
                    tmp['bidAmt'] = eachAdData['adAttr']['bidAmt']

                    ad_data.append(tmp)
            elif (eachGrpData['campaignTp'] == 'POWER_CONTENTS'):
                raw_kwd = GetAPIMethod.get_adkeyword_list(self.BASE_URL, self.API_KEY, self.SECRET_KEY, self.CUSTOMER_ID, eachGrpData['nccAdgroupId'])

                for eachKwdData in raw_kwd:
                    tmp = {}
                    tmp['id'] = eachKwdData['nccKeywordId']
                    tmp['nccAdgroupId'] = eachKwdData['nccAdgroupId']
                    tmp['type'] = 'easter_egg'
                    tmp['kwdName'] = eachKwdData['keyword']
                    tmp['bidAmt'] = eachKwdData['bidAmt']

                    kwd_data.append(tmp)
            else:
                easter_egg = '이외 캠페인은 지원하지 않음.'

        #################################
        # Keywords / Ads Ids 전처리 부
        #################################
        total_kwd_ids = []

        for each_id in kwd_data:
            kwd_ids = each_id['id']
            total_kwd_ids.append(kwd_ids)

        total_ad_ids = []

        for each_id in ad_data:
            kwd_ids = each_id['id']
            total_ad_ids.append(kwd_ids)

        total_kwd_ids = [total_kwd_ids[t:(t + 200)] for t in range(0, len(total_kwd_ids), 200)]
        total_ad_ids = [total_ad_ids[t:(t + 200)] for t in range(0, len(total_ad_ids), 200)]


        #################################
        # Keywords Stats Data 호출 부
        #################################

        kwd_stats_data = []

        for each_200 in total_kwd_ids:
            raw_stats = GetAPIMethod.get_adkeyword_stats_list(self.BASE_URL, self.API_KEY, self.SECRET_KEY, self.CUSTOMER_ID, each_200)
            raw_stats = raw_stats['data']

            for tmp in raw_stats:
                kwd_stats_data.append(tmp)


        #################################
        # Ads Stats Data 호출 부
        #################################

        ad_stats_data = []

        for each_200 in total_ad_ids:
            raw_stats = GetAPIMethod.get_adkeyword_stats_list(self.BASE_URL, self.API_KEY, self.SECRET_KEY, self.CUSTOMER_ID, each_200)
            raw_stats = raw_stats['data']

            for tmp in raw_stats:
                ad_stats_data.append(tmp)



        # Merge Keywords & Stats
        kwd_tmp = pd.DataFrame(kwd_data)
        ad_tmp = pd.DataFrame(ad_data)
        kwd_sts_tmp = pd.DataFrame(kwd_stats_data)
        ad_sts_tmp = pd.DataFrame(ad_stats_data)

        concat_data = pd.concat([kwd_tmp, ad_tmp], axis=0)
        concat_stats = pd.concat([kwd_sts_tmp, ad_sts_tmp], axis=0)

        merge_data = pd.merge(concat_data, concat_stats, how='outer')
        merge_data = merge_data.to_dict('records')

        return merge_data

    def make_adgroup_targets_list(self, get_grp_data):
        target_data = []

        for eachGrpData in get_grp_data:
            raw_target = GetAPIMethod.get_adgroup_targets_list(self.BASE_URL, self.API_KEY, self.SECRET_KEY, self.CUSTOMER_ID, eachGrpData['nccAdgroupId'])

            tmp = {}
            tmp['nccAdgroupId'] = raw_target[0]['ownerId']
            tmp['pc'] = raw_target[0]['target']['pc']
            tmp['mobile'] = raw_target[0]['target']['mobile']
            target_data.append(tmp)

        return target_data