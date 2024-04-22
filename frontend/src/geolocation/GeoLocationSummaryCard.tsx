// @flow
import * as React from 'react';
import { MediaGeoLocationSummary } from '../model/media-geo-location-summary';
import { Box, Card, CardContent, CardMedia, Typography } from '@mui/material';
import MmsIcon from '@mui/icons-material/Mms';

type Props = {
  geolocationSummary: MediaGeoLocationSummary;
  onClick: (region: string) => void;
};

export function GeoLocationSummaryCard({ geolocationSummary, onClick }: Props) {
  return (
    <Card
      key={`card-${geolocationSummary.region}`}
      onClick={() => onClick(geolocationSummary.region)}
    >
      <Box sx={{ position: 'relative' }} key={`media-box-1-${geolocationSummary.region}`}>
        {geolocationSummary.thumbnail && (
          <CardMedia
            key={`card-media-${geolocationSummary.region}`}
            component="img"
            width={geolocationSummary.randomPhotoThumbnailWidth}
            height={geolocationSummary.randomPhotoThumbnailHeight}
            image={`data:image/jpeg;base64,${geolocationSummary.thumbnail}`}
          ></CardMedia>
        )}
        {!geolocationSummary.thumbnail && <Box sx={{ width: '224px', height: '168px' }} />}
        <Box
          sx={{
            display: 'flex',
            alignItems: 'center',
            position: 'absolute',
            bottom: '10px',
            right: '10px',
            gap: '5px',
          }}
        >
          <Typography
            id={`typography-0-${geolocationSummary.region}`}
            sx={{
              fontSize: '10px',
              color: 'white',
              display: 'flex',
              alignItems: 'center',
            }}
          >
            <MmsIcon />
          </Typography>
          <Typography
            id={`typography-video-duration-${geolocationSummary.region}`}
            variant="body2"
            align="right"
          >
            {geolocationSummary.mediasCount}
          </Typography>
        </Box>
      </Box>
      <CardContent
        key={`card-content-1-${geolocationSummary.region}`}
        sx={{
          padding: '2px',
          '&:last-child': {
            paddingBottom: 0,
          },
        }}
      >
        <Typography key={`typography-1-${geolocationSummary.region}`} variant="body2" align="right">
          {geolocationSummary.region}, {geolocationSummary.country}
        </Typography>
      </CardContent>
    </Card>
  );
}
