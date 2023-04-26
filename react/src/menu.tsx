import './index.scss';

export default function Menu(): React.ReactNode {
    return <view className="main-menu">
        <view className="top-bar">
            <view className="spacer"></view>
            <view className="rule"></view>
        </view>
        <view className="content">
        </view>
        <view className="bottom-bar">
            <view className="rule"></view>
        </view>
    </view>;
}