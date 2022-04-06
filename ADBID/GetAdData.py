import time
import requests
import hashlib
import hmac
import base64

def get_header(method, uri, api_key, secret_key, customer_id):
    timestamp = str(round(time.time() * 1000))
    message = "{}.{}.{}".format(timestamp, method, uri)
    hash = hmac.new(bytes(secret_key, "utf-8"), bytes(message, "utf-8"), hashlib.sha256)
    hash.hexdigest()
    signature = base64.b64encode(hash.digest())
    return {'Content-Type': 'application/json; charset=UTF-8', 'X-Timestamp': timestamp, 'X-API-KEY': api_key,
            'X-Customer': str(customer_id), 'X-Signature': signature}

class GetAPIMethod:
    def get_campaign_list(base_url, api_key, secret_key, customer_id):
        uri = '/ncc/campaigns'
        method = 'GET'
        r = requests.get(base_url + uri, headers=get_header(method, uri, api_key, secret_key, customer_id))

        return r.json()

    def get_adgroup_list(base_url, api_key, secret_key, customer_id):
        uri = '/ncc/adgroups'
        method = 'GET'
        r = requests.get(base_url + uri, headers=get_header(method, uri, api_key, secret_key, customer_id))

        return r.json()

    def get_ad_list(base_url, api_key, secret_key, customer_id, ids):
        uri = '/ncc/ads'
        method = 'GET'
        r = requests.get(base_url + uri, params={'nccAdgroupId' :ids, 'recordSize' :1000},
                         headers=get_header(method, uri, api_key, secret_key, customer_id))

        return r.json()

    def get_adkeyword_list(base_url, api_key, secret_key, customer_id, ids):
        uri = '/ncc/keywords'
        method = 'GET'
        r = requests.get(base_url + uri, params={'nccAdgroupId' :ids, 'recordSize' :1000},
                         headers=get_header(method, uri, api_key, secret_key, customer_id))

        return r.json()

    def get_adkeyword_stats_list(base_url, api_key, secret_key, customer_id, ids):
        uri = '/stats'
        method = 'GET'
        r = requests.get(base_url + uri, params={'ids' :ids, 'fields': '["clkCnt","impCnt","pcNxAvgRnk", "mblNxAvgRnk"]', 'datePreset' : 'last7days'},
                         headers=get_header(method, uri, api_key, secret_key, customer_id))

        return r.json()

    def get_adgroup_targets_list(base_url, api_key, secret_key, customer_id, ids):
        uri = '/ncc/targets'
        method = 'GET'
        r = requests.get(base_url + uri, params={'ownerId': ids, 'types' :['PC_MOBILE_TARGET']},
                         headers=get_header(method, uri, api_key, secret_key, customer_id))

        return r.json()