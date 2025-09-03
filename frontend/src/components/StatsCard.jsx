import React from 'react';
import { Card, CardContent, CardHeader, CardTitle } from './ui/card';

const StatsCard = ({ title, value, subtitle, icon, trend, trendValue }) => {
  const getTrendColor = (trend) => {
    if (trend === 'up') return 'text-green-600';
    if (trend === 'down') return 'text-red-600';
    return 'text-gray-600';
  };

  const getTrendIcon = (trend) => {
    if (trend === 'up') return '↗';
    if (trend === 'down') return '↘';
    return '→';
  };

  return (
    <Card>
      <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-2">
        <CardTitle className="text-sm font-medium">{title}</CardTitle>
        {icon && <div className="text-2xl">{icon}</div>}
      </CardHeader>
      <CardContent>
        <div className="text-2xl font-bold">{value}</div>
        <div className="flex items-center justify-between">
          {subtitle && <p className="text-xs text-muted-foreground">{subtitle}</p>}
          {trend && (
            <div className={`flex items-center text-xs ${getTrendColor(trend)}`}>
              <span className="mr-1">{getTrendIcon(trend)}</span>
              {trendValue}
            </div>
          )}
        </div>
      </CardContent>
    </Card>
  );
};

export default StatsCard;