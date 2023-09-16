import { PageContainer } from "@ant-design/pro-components";
import { Button, App } from "antd";


export default function Product() {
    const { message } = App.useApp();
    return <PageContainer content="SPU" >
        product
        <Button onClick={() => { message.success("info") }}>message.info</Button>
    </PageContainer>;
}