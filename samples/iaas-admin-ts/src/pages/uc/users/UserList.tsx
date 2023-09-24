import { ActionType, PageContainer, ProColumns, ProTable, TableDropdown } from "@ant-design/pro-components";
import request from "../../../utils/request";
import { Button, Dropdown } from "antd";
import { useRef } from "react";
import { PlusOutlined, EllipsisOutlined } from '@ant-design/icons'

type UserType = { id: number, real_name: string, deleted: number, created_at: string, creator_name: string, last_modified_at?: string }


const columns: ProColumns<UserType>[] = [
    {
        dataIndex: 'index',
        valueType: 'indexBorder',
        width: 48,
    },
    {
        title: '姓名',
        dataIndex: 'real_name',
        copyable: true,
        ellipsis: true,
        tip: '过长会自动收缩',
        formItemProps: {
            rules: [
                {
                    required: true,
                    message: '此项为必填项',
                },
            ],
        },
    },
    {
        disable: true,
        title: '状态',
        dataIndex: 'deleted',
        filters: true,
        onFilter: true,
        ellipsis: true,
        valueType: 'select',
        valueEnum: {
            all: { text: '全部' },
            open: {
                text: 'true',
                status: 'Error',
            },
            closed: {
                text: 'false',
                status: 'Success',
            },
            processing: {
                text: '解决中',
                status: 'Processing',
                disabled: true,
            },
        },
    },
    // {
    //   disable: true,
    //   title: '标签',
    //   dataIndex: 'labels',
    //   search: false,
    //   renderFormItem: (_, { defaultRender }) => {
    //     return defaultRender(_);
    //   },
    //   render: (_, record) => (
    //     <Space>
    //       {record.labels.map(({ name, color }) => (
    //         <Tag color={color} key={name}>
    //           {name}
    //         </Tag>
    //       ))}
    //     </Space>
    //   ),
    // },
    {
        title: '创建时间',
        key: 'showTime',
        dataIndex: 'created_at',
        valueType: 'date',
        sorter: true,
        hideInSearch: true,
    },
    {
        title: '创建时间',
        dataIndex: 'created_at',
        valueType: 'dateRange',
        hideInTable: true,
        search: {
            transform: (value) => {
                return {
                    startTime: value[0],
                    endTime: value[1],
                };
            },
        },
    },
    {
        title: '创建人',
        key: 'showTime',
        dataIndex: 'creator_name',
        sorter: true,
        hideInSearch: true,
    },
    {
        title: '最后修改时间',
        key: 'showTime',
        dataIndex: 'last_modified_at',
        valueType: 'date',
        sorter: true,
        hideInSearch: true,
    },
    {
        title: '操作',
        valueType: 'option',
        key: 'option',
        render: (text, record, _, action) => [
            <a
                key="editable"
                onClick={() => {
                    action?.startEditable?.(record.id);
                }}
            >
                编辑
            </a>,
            // <a href={record.url} target="_blank" rel="noopener noreferrer" key="view">
            //   查看
            // </a>,
            <TableDropdown
                key="actionGroup"
                onSelect={() => action?.reload()}
                menus={[
                    { key: 'copy', name: '复制' },
                    { key: 'delete', name: '删除' },
                ]}
            />,
        ],
    },
];
export default function Users() {
    const actionRef = useRef<ActionType>();
    
    return <PageContainer title="用户管理" content="新建用户、重置密码、锁定账号、账号解锁、登录限制等">
        <ProTable<UserType>
            request={async (
                // 第一个参数 params 查询表单和 params 参数的结合
                // 第一个参数中一定会有 pageSize 和  current ，这两个参数是 antd 的规范
                params: {
                    // pageSize: number;
                    // current: number;
                },
                sort,
                filter,
            ) => {
                // 这里需要返回一个 Promise,在返回之前你可以进行数据转化
                // 如果需要转化参数可以在这里进行修改
                // const msg = await myQuery({
                //     page: params.current,
                //     pageSize: params.pageSize,
                // });
                console.log(params);
                
                let rt = await request({ url: '/api/users/all', method: 'get' });
                return {
                    data: rt.data,
                    // success 请返回 true，
                    // 不然 table 会停止解析数据，即使有数据
                    success: rt.code === 200,
                    // 不传会使用 data 的长度，如果是分页一定要传
                    // total: number,
                };
            }}
            columns={columns}
            editable={{
                type: 'multiple',
            }}
            columnsState={{
                persistenceKey: 'pro-table-singe-demos',
                persistenceType: 'localStorage',
                onChange(value) {
                    console.log('value: ', value);
                },
            }}
            rowKey="Id"
            search={{
                labelWidth: 'auto',
            }}
            options={{
                setting: {
                    listsHeight: 400,
                },
            }}
            form={{
                // 由于配置了 transform，提交的参与与定义的不同这里需要转化一下
                syncToUrl: (values, type) => {
                    if (type === 'get') {
                        return {
                            ...values,
                            created_at: [values.startTime, values.endTime],
                        };
                    }
                    return values;
                },
            }}
            pagination={{
                pageSize: 5,
                onChange: (page) => console.log(page),
            }}
            dateFormatter="string"
            headerTitle="高级表格"
            toolBarRender={() => [
                <Button
                    key="button"
                    icon={<PlusOutlined />}
                    onClick={() => {
                        actionRef.current?.reload();
                    }}
                    type="primary"
                >
                    新建
                </Button>,
                <Dropdown
                    key="menu"
                    menu={{
                        items: [
                            {
                                label: '重置密码',
                                key: 'reset_password',
                            },
                            {
                                label: '锁定',
                                key: 'lock',
                            },
                            {
                                label: '解锁',
                                key: 'unlock',
                            },
                        ],
                    }}
                >
                    <Button>
                        <EllipsisOutlined />
                    </Button>
                </Dropdown>,
            ]}></ProTable>
    </PageContainer>
}

