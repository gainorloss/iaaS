import axios from "axios";

axios.defaults.timeout = 2000;
axios.defaults.headers.common["Authorization"] = "erp";
axios.defaults.headers.post['Content-Type'] = 'application/json';
axios.interceptors.request.use(config => {
    // console.log(`config:${JSON.stringify(config)}`);
    return config;
});
axios.interceptors.response.use(res => {
    // console.log(`res:${JSON.stringify(res)}`);
    return res;
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