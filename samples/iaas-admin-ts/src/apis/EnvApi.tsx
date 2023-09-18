import request from "../utils/request";

export default async function EnvApi(): Promise<{ HOSTNAME: string, processor_count: string }> {
    const res = await request({ url: '/api/env', method: 'get' });
    const { HOSTNAME, processor_count } = res.data;
    return { HOSTNAME, processor_count };
}
