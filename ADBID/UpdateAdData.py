import time
import requests
import hashlib
import hmac
import base64
import jsonpickle
import os
from GetAdData import GetAPIMethod

def get_header(method, uri, api_key, secret_key, customer_id):
    timestamp = str(round(time.time() * 1000))
    message = "{}.{}.{}".format(timestamp, method, uri)
    hash = hmac.new(bytes(secret_key, "utf-8"), bytes(message, "utf-8"), hashlib.sha256)
    hash.hexdigest()
    signature = base64.b64encode(hash.digest())
    return {'Content-Type': 'application/json; charset=UTF-8', 'X-Timestamp': timestamp, 'X-API-KEY': api_key,
            'X-Customer': str(customer_id), 'X-Signature': signature}


class UpdateAPIMethod:
    def __init__(self, base_url, api_key, secret_key, customer_id):
        self.BASE_URL = base_url
        self.API_KEY = api_key
        self.SECRET_KEY = secret_key
        self.CUSTOMER_ID = customer_id

#    def post_position_bid(self, keywordData):
#        uri = '/estimate/average-position-bid/id'
#        method = 'POST'
#
#        data = {"device": keywordData['device'], "items": [{"key": keywordData['id'], "position": keywordData['position']}]}
#        fields_data = jsonpickle.encode(data, unpicklable=False)
#        r = requests.post(self.BASE_URL + uri, data=fields_data, headers=get_header(method, uri, self.API_KEY, self.SECRET_KEY, self.CUSTOMER_ID))
#
#        return r.json()['estimate'][0]['bid']

    def update_keyword_bid(self, input_data, output_data):
        for eachInput in input_data:
            if (eachInput['campaignTp'] != 'SHOPPING') :

                #################################################
                # -- Position Bid 호출 부
                #################################################
                uri_post = '/estimate/average-position-bid/id'
                method_post = 'POST'

                data = {"device": 'pc', "items": [{"key": eachInput['id'], "position": eachInput['position']}]}
                fields_data = jsonpickle.encode(data, unpicklable=False)
                r_post = requests.post(self.BASE_URL + uri_post, data=fields_data,
                                  headers=get_header(method_post, uri_post, self.API_KEY, self.SECRET_KEY, self.CUSTOMER_ID))

                posBid = r_post.json()['estimate'][0]['bid']


                #################################################
                # -- Keyword Bid Update 호출 부
                #################################################
                uri = '/ncc/keywords/' + eachInput['id']
                method = 'PUT'

                data = {"bidAmt": min((posBid + eachInput['addBidAmt']), eachInput['maxBidAmt']), "nccAdgroupId": eachInput['nccAdgroupId'], "nccKeywordId": eachInput['id'], "useGroupBidAmt": False}
                fields_data = jsonpickle.encode(data, unpicklable=False)

                r = requests.put(self.BASE_URL + uri + '?fields=bidAmt', data=fields_data, headers=get_header(method, uri, self.API_KEY, self.SECRET_KEY, self.CUSTOMER_ID))

                output_data[eachInput['id']] = r.json()
            else :
                #################################################
                # -- Position Bid 호출 부
                #################################################
                uri_post = '/npla-estimate/average-position-bid/id'
                method_post = 'POST'

                data = {"device": 'pc',
                        "items": [{"key": eachInput['id'], "position": eachInput['position']}]}
                fields_data = jsonpickle.encode(data, unpicklable=False)
                r_post = requests.post(self.BASE_URL + uri_post, data=fields_data,
                                       headers=get_header(method_post, uri_post, self.API_KEY, self.SECRET_KEY,
                                                          self.CUSTOMER_ID))

                posBid = r_post.json()['estimate'][0]['bid']

                #################################################
                # -- AdGroup Bid Update 호출 부
                #################################################
                uri = '/ncc/ads/' + eachInput['id']
                method = 'PUT'

                data = {"type": eachInput['type'], "adAttr": {"bidAmt": min((posBid + eachInput['addBidAmt']), eachInput['maxBidAmt']),
                        "useGroupBidAmt": False}, "nccAdId": eachInput['id']}
                fields_data = jsonpickle.encode(data, unpicklable=False)

                r = requests.put(self.BASE_URL + uri + '?fields=adAttr', data=fields_data,
                                 headers=get_header(method, uri, self.API_KEY, self.SECRET_KEY, self.CUSTOMER_ID))

                output_data[eachInput['id']] = r.json()

    def update_keyword_stats(self, input_data, output_data):
        #################################
        # -- input_data에서 kwd / ad 200 분류
        #################################
        total_kwd_ids = []
        total_ad_ids = []

        for each_id in input_data:
            if (each_id['campaignTp'] != 'SHOPPING'):
                total_kwd_ids.append(each_id['id'])
            else:
                total_ad_ids.append(each_id['id'])

        total_kwd_ids = [total_kwd_ids[t:(t + 200)] for t in range(0, len(total_kwd_ids), 200)]
        total_ad_ids = [total_ad_ids[t:(t + 200)] for t in range(0, len(total_ad_ids), 200)]

        #################################
        # Keywords Stats Data 호출 부
        #################################
        for each_200 in total_kwd_ids:
            raw_stats = GetAPIMethod.get_adkeyword_stats_list(self.BASE_URL, self.API_KEY, self.SECRET_KEY,
                                                              self.CUSTOMER_ID, each_200)
            raw_stats = raw_stats['data']

            for tmp in raw_stats:
                output_data[tmp['id']] = tmp

        #################################
        # Ads Stats Data 호출 부
        #################################
        for each_200 in total_ad_ids:
            raw_stats = GetAPIMethod.get_adkeyword_stats_list(self.BASE_URL, self.API_KEY, self.SECRET_KEY,
                                                              self.CUSTOMER_ID, each_200)
            raw_stats = raw_stats['data']

            for tmp in raw_stats:
                output_data[tmp['id']] = tmp



