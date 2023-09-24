import { router } from "@/router";
import axios from "axios";


axios.defaults.timeout = 2000;
// axios.defaults.headers.common["Authorization"] = "erp";
axios.defaults.headers.post['Content-Type'] = 'application/json';
axios.interceptors.request.use(config => {
    // console.log(`config:${JSON.stringify(config)}`);
    const token = localStorage.getItem("access_token");
    if (token) {
        config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
}, err => {
    return Promise.reject(err);
});
axios.interceptors.response.use(res => {
    // console.log(`res:${JSON.stringify(res)}`);
    return res;
}, error => {
    console.log(error);
    if (error.response) {
        switch (error.response.status) {
            case 401:
                // 返回 401 清除token信息并跳转到登录页面
                // store.commit(types.LOGOUT);
                // router.replace({
                //     path: 'login',
                //     query: { redirect: router.currentRoute.fullPath }
                // })
                localStorage.removeItem('access_token');
                window.location.hash=`login?redirect=/uc/users`;
        }
    }
    return Promise.reject(error);
});

export default async function request(params: ({ url: string, method: string, data?: any })): Promise<{ code: number, msg?: string, data?: any }> {
    var para = {
        ...params,
        validateStatus: (status: number) => {
            console.log(`"${params.url}"：${status}`);
            return status >= 200 && status < 300; // default
        }
    };
    var res = await axios(para);
    if (res.status != 200) {
        return { code: 500, msg: `网络异常，请重试` }
    }
    var data = res.data;
    return { ...data };
}